﻿using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

public class Tier
{
    public Tier(TierId id, string name)
    {
        Id = id;
        Name = name;
        Quotas = new();
    }

    public TierId Id { get; }
    public string Name { get; }
    public List<TierQuotaDefinition> Quotas { get; }

    public Result<TierQuotaDefinition, DomainError> CreateQuota(MetricKey metricKey, int max, QuotaPeriod period)
    {
        if (max <= 0)
            return Result.Failure<TierQuotaDefinition, DomainError>(DomainErrors.TierQuotaMaxValueCannotBeLowerOrEqualToZero());

        var quotaDefinition = new TierQuotaDefinition(metricKey, max, period);
        Quotas.Add(quotaDefinition);

        return Result.Success<TierQuotaDefinition, DomainError>(quotaDefinition);
    }

    public Result<TierQuotaDefinitionId, DomainError> DeleteQuota(string tierQuotaDefinitionId)
    {
        var quotaDefinition = Quotas.FirstOrDefault(q => q.Id == tierQuotaDefinitionId);

        if (quotaDefinition == null)
            return Result.Failure<TierQuotaDefinitionId, DomainError>(GenericDomainErrors.NotFound(nameof(TierQuotaDefinition)));

        Quotas.Remove(quotaDefinition);

        return Result.Success<TierQuotaDefinitionId, DomainError>(quotaDefinition.Id);
    }
}
