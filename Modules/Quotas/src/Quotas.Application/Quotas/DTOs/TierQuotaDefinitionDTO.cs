using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;

namespace Backbone.Modules.Quotas.Application.Quotas.DTOs;

public class TierQuotaDefinitionDTO
{
    public TierQuotaDefinitionDTO(TierQuotaDefinitionId id, Metric metric, int max, QuotaPeriod period)
    {
        Id = id;
        Metric = metric;
        Max = max;
        Period = period;
    }

    public string Id { get; set; }
    public Metric Metric { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
