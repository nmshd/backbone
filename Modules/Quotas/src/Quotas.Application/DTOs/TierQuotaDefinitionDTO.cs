using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class TierQuotaDefinitionDTO : IMapTo<TierQuotaDefinition>
{
    private TierQuotaDefinitionDTO() { }

    public TierQuotaDefinitionDTO(string id, MetricDTO metric, int max, QuotaPeriod period)
    {
        Id = id;
        Metric = metric;
        Max = max;
        Period = period;
    }

    public string Id { get; set; }
    public MetricDTO Metric { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}