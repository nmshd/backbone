using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class TierQuotaDefinitionDTO
{
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
