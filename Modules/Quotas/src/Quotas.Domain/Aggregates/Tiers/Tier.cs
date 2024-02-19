using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class Tier
{
    public static readonly Tier QUEUED_FOR_DELETION = new(new TierId("TIR00000000000000001"), "Queued For Deletion");

    public Tier(TierId id, string name)
    {
        Id = id;
        Name = name;
        Quotas = new List<TierQuotaDefinition>();
    }

    public TierId Id { get; }
    public string Name { get; }
    public List<TierQuotaDefinition> Quotas { get; }

    public Result<TierQuotaDefinition, DomainError> CreateQuota(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (IsQueuedForDeletionTier())
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.CannotCreateOrDeleteQuotaForQueuedForDeletionTier());

        if (max < 0)
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.MaxValueCannotBeLowerThanZero());

        return AddTierQuotaDefinition(metricKey, max, period);
    }

    public Result<TierQuotaDefinitionId, DomainError> DeleteQuota(string tierQuotaDefinitionId)
    {
        if (IsQueuedForDeletionTier())
            return Result.Failure<TierQuotaDefinitionId, DomainError>(DomainErrors.CannotCreateOrDeleteQuotaForQueuedForDeletionTier());

        var quotaDefinition = Quotas.FirstOrDefault(q => q.Id == tierQuotaDefinitionId);

        if (quotaDefinition == null)
            return Result.Failure<TierQuotaDefinitionId, DomainError>(GenericDomainErrors.NotFound(nameof(TierQuotaDefinition)));

        Quotas.Remove(quotaDefinition);

        return Result.Success<TierQuotaDefinitionId, DomainError>(quotaDefinition.Id);
    }

    public IEnumerable<TierQuotaDefinition> AddQuotaForAllMetricsOnQueuedForDeletion(IEnumerable<Metric> metrics)
    {
        if (!IsQueuedForDeletionTier())
            throw new InvalidOperationException("Method can only be called for the 'Queued for Deletion' tier");

        var missingMetrics = metrics.Where(metric => Quotas.All(q => q.MetricKey.Value != metric.Key.Value));

        var addedQuotas = new List<TierQuotaDefinition>();
        foreach (var metric in missingMetrics)
        {
            var result = AddTierQuotaDefinition(metric.Key, 0, QuotaPeriod.Total);
            addedQuotas.Add(result.Value);
        }

        return addedQuotas;
    }

    private Result<TierQuotaDefinition, DomainError> AddTierQuotaDefinition(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (TierQuotaAlreadyExists(metricKey, period))
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.DuplicateQuota());

        var quotaDefinition = new TierQuotaDefinition(metricKey, max, period);
        Quotas.Add(quotaDefinition);

        return Result.Success<TierQuotaDefinition, DomainError>(quotaDefinition);
    }

    private bool IsQueuedForDeletionTier()
    {
        return Id == QUEUED_FOR_DELETION.Id;
    }

    private bool TierQuotaAlreadyExists(MetricKey metricKey, QuotaPeriod period)
    {
        return Quotas.Any(q => q.MetricKey == metricKey && q.Period == period);
    }
}
