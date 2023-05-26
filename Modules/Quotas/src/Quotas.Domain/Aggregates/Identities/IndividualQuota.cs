using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Identities;

public class IndividualQuota : Quota
{
    public IndividualQuota(MetricKey metricKey, int max, QuotaPeriod period)
    {
        MetricKey = metricKey;
        Max = max;
        Period = period;
    }

    public int Weight => 2;
    public MetricKey MetricKey { get; }
    public int Max { get; }
    public QuotaPeriod Period { get; }
}
