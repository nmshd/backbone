using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

public class Metric
{
    public Metric(MetricKey key, string displayName)
    {
        Key = key;
        DisplayName = displayName;  
    }
    public MetricKey Key { get; set; }

    public string DisplayName { get; set; }

    public MetricStatus MetricStatus { get; set; }
}
