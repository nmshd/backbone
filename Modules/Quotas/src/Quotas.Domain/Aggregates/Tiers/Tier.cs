using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class Tier
{
    public static readonly Tier UP_FOR_DELETION = new(new TierId("TIR00000000000000001"), "Up For Deletion");

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
        if (IsUpForDeletionTier())
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.CannotCreateOrDeleteQuotaForUpForDeletionTier());

        if (max < 0)
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.MaxValueCannotBeLowerThanZero());

        return CreateTierQuotaDefinition(metricKey, max, period);
    }

    public Result<TierQuotaDefinitionId, DomainError> DeleteQuota(string tierQuotaDefinitionId)
    {
        if (IsUpForDeletionTier())
            return Result.Failure<TierQuotaDefinitionId, DomainError>(DomainErrors.CannotCreateOrDeleteQuotaForUpForDeletionTier());

        var quotaDefinition = Quotas.FirstOrDefault(q => q.Id == tierQuotaDefinitionId);

        if (quotaDefinition == null)
            return Result.Failure<TierQuotaDefinitionId, DomainError>(GenericDomainErrors.NotFound(nameof(TierQuotaDefinition)));

        Quotas.Remove(quotaDefinition);

        return Result.Success<TierQuotaDefinitionId, DomainError>(quotaDefinition.Id);
    }

    public Result<TierQuotaDefinition, DomainError> CreateQuotaForUpForDeletionTier(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (!IsUpForDeletionTier())
            throw new InvalidOperationException("Method can only be called for the 'Up for Deletion' tier");

        return CreateTierQuotaDefinition(metricKey, max, period);
    }

    public IEnumerable<Result<TierQuotaDefinition, DomainError>> CreateQuotaForAllMetrics(IEnumerable<Metric> metrics)
    {
        var missingMetrics = metrics.Where(metric => Quotas.All(quota => quota.MetricKey.Value != metric.Key.Value));
        return missingMetrics.Select(metric => CreateQuotaForUpForDeletionTier(metric.Key, 0, QuotaPeriod.Total));
    }

    private Result<TierQuotaDefinition, DomainError> CreateTierQuotaDefinition(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (TierQuotaAlreadyExists(metricKey, period))
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.DuplicateQuota());

        var quotaDefinition = new TierQuotaDefinition(metricKey, max, period);
        Quotas.Add(quotaDefinition);

        return Result.Success<TierQuotaDefinition, DomainError>(quotaDefinition);
    }

    private bool IsUpForDeletionTier()
    {
        return Id == UP_FOR_DELETION.Id;
    }

    private bool TierQuotaAlreadyExists(MetricKey metricKey, QuotaPeriod period)
    {
        return Quotas.Any(q => q.MetricKey == metricKey && q.Period == period);
    }
}
