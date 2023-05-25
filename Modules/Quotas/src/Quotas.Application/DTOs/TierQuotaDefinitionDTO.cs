using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class TierQuotaDefinitionDTO : IMapTo<TierQuotaDefinition>
{
    public string Id { get; set; }
    public MetricKey MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
