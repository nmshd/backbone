using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.Tooling;
using CSharpFunctionalExtensions;
using Entity = Backbone.BuildingBlocks.Domain.Entity;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity : Entity
{
    private readonly List<TierQuota> _tierQuotas;
    private readonly List<IndividualQuota> _individualQuotas;
    private readonly List<MetricStatus> _metricStatuses;

    private readonly Lock _latestExhaustionDateLock = new();

    // ReSharper disable once UnusedMember.Local
    protected Identity()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Address = null!;
        _tierQuotas = null!;
        _individualQuotas = null!;
        _metricStatuses = null!;
        TierId = null!;
    }

    public Identity(string address, TierId tierId)
    {
        Address = address;
        TierId = tierId;
        _tierQuotas = [];
        _individualQuotas = [];
        _metricStatuses = [];
    }

    public string Address { get; }

    public TierId TierId { get; private set; }

    public virtual IReadOnlyCollection<MetricStatus> MetricStatuses => _metricStatuses.AsReadOnly();

    public virtual IReadOnlyCollection<TierQuota> TierQuotas => _tierQuotas.AsReadOnly();
    public virtual IReadOnlyCollection<IndividualQuota> IndividualQuotas => _individualQuotas.AsReadOnly();
    internal virtual IReadOnlyCollection<Quota> AllQuotas => new List<Quota>(_individualQuotas).Concat(new List<Quota>(_tierQuotas)).ToList().AsReadOnly();

    public IndividualQuota CreateIndividualQuota(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (max < 0)
            throw new DomainException(DomainErrors.MaxValueCannotBeLowerThanZero());

        if (IndividualQuotaAlreadyExists(metricKey, period))
            throw new DomainException(DomainErrors.DuplicateQuota());

        var individualQuota = new IndividualQuota(metricKey, max, period, Address);
        _individualQuotas.Add(individualQuota);

        return individualQuota;
    }

    public Result<QuotaId, DomainError> DeleteIndividualQuota(QuotaId individualQuotaId)
    {
        var individualQuota = _individualQuotas.FirstOrDefault(q => q.Id == individualQuotaId);

        if (individualQuota == null)
            return Result.Failure<QuotaId, DomainError>(GenericDomainErrors.NotFound(nameof(IndividualQuota)));

        _individualQuotas.Remove(individualQuota);

        return Result.Success<QuotaId, DomainError>(individualQuota.Id);
    }

    public void AssignTierQuotaFromDefinition(TierQuotaDefinition definition)
    {
        var tierQuota = new TierQuota(definition, Address);
        _tierQuotas.Add(tierQuota);
    }

    public void DeleteTierQuotaFromDefinitionId(TierQuotaDefinitionId tierQuotaDefinitionId)
    {
        var tierQuota = _tierQuotas.FirstOrDefault(tq => tq.DefinitionId == tierQuotaDefinitionId)
                        ?? throw new DomainException(GenericDomainErrors.NotFound(nameof(TierQuotaDefinition)));

        _tierQuotas.Remove(tierQuota);
    }

    public async Task UpdateMetricStatuses(IEnumerable<MetricKey> metrics, MetricCalculatorFactory factory, MetricUpdateType updateType, CancellationToken cancellationToken)
    {
        foreach (var metric in metrics)
        {
            var metricCalculator = factory.CreateFor(metric);
            await UpdateMetricStatus(metric, metricCalculator, updateType, cancellationToken);
        }
    }

    private async Task UpdateAllMetricStatuses(MetricCalculatorFactory factory, MetricUpdateType updateType, CancellationToken cancellationToken)
    {
        var metricKeys = _tierQuotas.Select(q => q.MetricKey).Union(_individualQuotas.Select(q => q.MetricKey)).Distinct();
        await UpdateMetricStatuses(metricKeys, factory, updateType, cancellationToken);
    }

    private bool IndividualQuotaAlreadyExists(MetricKey metricKey, QuotaPeriod period)
    {
        return _individualQuotas.Any(q => q.MetricKey == metricKey && q.Period == period);
    }

    private async Task UpdateMetricStatus(MetricKey metric, IMetricCalculator metricCalculator, MetricUpdateType updateType, CancellationToken cancellationToken)
    {
        var metricStatus = _metricStatuses.SingleOrDefault(m => m.MetricKey == metric);

        if (updateType == MetricUpdateType.OnlyExhausted && metricStatus is { IsExhausted: false })
            return;

        var quotasForMetric = GetAppliedQuotasForMetric(metric);

        var latestExhaustionDate = ExhaustionDate.UNEXHAUSTED;

        var utcNow = SystemTime.UtcNow;

        await Parallel.ForEachAsync(quotasForMetric, cancellationToken, async (quota, _) =>
        {
            // if the quota allows 0, we don't need to calculate the usage, it's always exhausted
            // this is primarily an optimization for when an identity moves to the QueuedForDeletion tier, where all quotas are set to 0
            var newUsage = quota.Max > 0
                ? 1
                : await metricCalculator.CalculateUsage(
                    quota.Period.CalculateBegin(utcNow),
                    quota.Period.CalculateEnd(utcNow),
                    Address,
                    cancellationToken);

            var quotaExhaustion = quota.CalculateExhaustion(newUsage, utcNow);

            lock (_latestExhaustionDateLock)
            {
                if (quotaExhaustion > latestExhaustionDate)
                    latestExhaustionDate = quotaExhaustion;
            }
        });

        if (metricStatus != null)
            metricStatus.Update(latestExhaustionDate);
        else
            _metricStatuses.Add(new MetricStatus(metric, Address, latestExhaustionDate));
    }

    private IEnumerable<Quota> GetAppliedQuotasForMetric(MetricKey metric)
    {
        var allQuotasOfMetric = AllQuotas.Where(q => q.MetricKey == metric).ToArray();

        if (allQuotasOfMetric.Length == 0)
            return [];

        var highestWeight = allQuotasOfMetric.Max(q => q.Weight);
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight).ToArray();
        return appliedQuotas;
    }

    public async Task ChangeTier(Tier newTier, MetricCalculatorFactory metricCalculatorFactory, CancellationToken cancellationToken)
    {
        if (TierId == newTier.Id)
            throw new DomainException(GenericDomainErrors.NewAndOldParametersMatch("TierId"));

        _tierQuotas.Clear();
        _metricStatuses.Clear();

        TierId = newTier.Id;
        foreach (var tierQuotaDefinition in newTier.Quotas)
        {
            AssignTierQuotaFromDefinition(tierQuotaDefinition);
        }

        await UpdateAllMetricStatuses(metricCalculatorFactory, MetricUpdateType.All, cancellationToken);
    }

    #region Selectors

    public static Expression<Func<Identity, bool>> HasAddress(IdentityAddress identityAddress)
    {
        return i => i.Address == identityAddress.ToString();
    }

    #endregion
}

public enum MetricUpdateType
{
    All,
    OnlyExhausted
}
