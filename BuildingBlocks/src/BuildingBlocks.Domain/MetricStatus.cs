using Backbone.Tooling;

namespace Backbone.BuildingBlocks.Domain;
public class MetricStatus
{
    public bool IsExhausted => IsExhaustedUntil.HasValue && IsExhaustedUntil > SystemTime.UtcNow;

    private MetricStatus()
    {
        MetricKey = null!;
        Owner = null!;
    }

    internal MetricStatus(MetricKey metricKey, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
        Owner = null!;
    }

    public MetricKey MetricKey { get; }
    public DateTime? IsExhaustedUntil { get; }

    public string Owner { get; }
}
