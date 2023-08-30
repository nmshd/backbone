using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain;
using Enmeshed.BuildingBlocks.Domain.Errors;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class Identity
{
    private readonly List<TierQuota> _tierQuotas;
    private readonly List<IndividualQuota> _individualQuotas;
    private readonly List<MetricStatus> _metricStatuses;

    public Identity(string address, TierId tierId)
    {
        Address = address;
        TierId = tierId;
        _tierQuotas = new List<TierQuota>();
        _individualQuotas = new List<IndividualQuota>();
        _metricStatuses = new List<MetricStatus>();
    }
    private Identity() { }

    public IReadOnlyCollection<MetricStatus> MetricStatuses => _metricStatuses.AsReadOnly();
    public IReadOnlyCollection<TierQuota> TierQuotas => _tierQuotas.AsReadOnly();
    public IReadOnlyCollection<IndividualQuota> IndividualQuotas => _individualQuotas.AsReadOnly();
    internal IReadOnlyCollection<Quota> AllQuotas => new List<Quota>(_individualQuotas).Concat(new List<Quota>(_tierQuotas)).ToList().AsReadOnly();

    public string Address { get; }
    public TierId TierId { get; }

    public IndividualQuota CreateIndividualQuota(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (max <= 0)
            throw new DomainException(DomainErrors.MaxValueCannotBeLowerOrEqualToZero());

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

    public async Task UpdateMetricStatuses(IEnumerable<MetricKey> metrics, MetricCalculatorFactory factory,
        CancellationToken cancellationToken)
    {
        foreach (var metric in metrics)
        {
            var metricCalculator = factory.CreateFor(metric);
            await UpdateMetricStatus(metric, metricCalculator, cancellationToken);
        }
    }

    private bool IndividualQuotaAlreadyExists(MetricKey metricKey, QuotaPeriod period)
    {
        return _individualQuotas.Any(q => q.MetricKey == metricKey && q.Period == period);
    }

    private async Task UpdateMetricStatus(MetricKey metric, IMetricCalculator metricCalculator,
        CancellationToken cancellationToken)
    {
        var quotasForMetric = GetAppliedQuotasForMetric(metric);

        var latestExhaustionDate = ExhaustionDate.Unexhausted;

        await Parallel.ForEachAsync(quotasForMetric, cancellationToken, async (quota, _) =>
        {
            var newUsage = await metricCalculator.CalculateUsage(
                quota.Period.CalculateBegin(),
                quota.Period.CalculateEnd(),
                Address,
                cancellationToken);

            var quotaExhaustion = quota.CalculateExhaustion(newUsage);

            lock (latestExhaustionDate)
            {
                if (quotaExhaustion > latestExhaustionDate)
                    latestExhaustionDate = quotaExhaustion;
            }
        });

        var metricStatus = _metricStatuses.SingleOrDefault(m => m.MetricKey == metric);
        if (metricStatus != null)
            metricStatus.Update(latestExhaustionDate);
        else
            _metricStatuses.Add(new MetricStatus(metric, Address, latestExhaustionDate));
    }

    private IEnumerable<Quota> GetAppliedQuotasForMetric(MetricKey metric)
    {
        var allQuotasOfMetric = AllQuotas.Where(q => q.MetricKey == metric);
        if (!allQuotasOfMetric.Any())
        {
            return Enumerable.Empty<Quota>();
        }

        var highestWeight = allQuotasOfMetric.Max(q => q.Weight);
        var appliedQuotas = allQuotasOfMetric.Where(q => q.Weight == highestWeight).ToArray();
        return appliedQuotas;
    }
}
