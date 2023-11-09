using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using CSharpFunctionalExtensions;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class Tier
{
    public const string UP_FOR_DELETION_TIER_NAME = "Up For Deletion";

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

        if (max <= 0)
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.MaxValueCannotBeLowerOrEqualToZero());

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
        return Id == TierId.UP_FOR_DELETION_DEFAULT_ID;
    }

    private bool TierQuotaAlreadyExists(MetricKey metricKey, QuotaPeriod period)
    {
        return Quotas.Any(q => q.MetricKey == metricKey && q.Period == period);
    }
}
