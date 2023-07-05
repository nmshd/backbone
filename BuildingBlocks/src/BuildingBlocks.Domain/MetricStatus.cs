using Enmeshed.Tooling;

namespace Enmeshed.BuildingBlocks.Domain;
public class MetricStatus
{
    public bool IsExhausted => IsExhaustedUntil.HasValue && IsExhaustedUntil > SystemTime.UtcNow;

    private MetricStatus()
    {
    }

    internal MetricStatus(MetricKey metricKey, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public MetricKey MetricKey { get; }
    public DateTime? IsExhaustedUntil { get; }

    public string Owner { get; }
}
