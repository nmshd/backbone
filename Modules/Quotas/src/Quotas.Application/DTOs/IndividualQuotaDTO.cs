using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class IndividualQuotaDTO
{
    public IndividualQuotaDTO() { }

    public IndividualQuotaDTO(string id, MetricKey metricKey, int max, QuotaPeriod period)
    {
        Id = id;
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public string Id { get; set; }
    public MetricKey MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}
