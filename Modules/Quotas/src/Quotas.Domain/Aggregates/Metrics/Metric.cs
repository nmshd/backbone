namespace Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

public class Metric
{
    public Metric(MetricKey key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public MetricKey Key { get; }
    public string DisplayName { get; }
}
