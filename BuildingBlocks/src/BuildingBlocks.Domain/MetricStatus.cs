using Backbone.Tooling;

namespace Backbone.BuildingBlocks.Domain;

public class MetricStatus
{
    public bool IsExhausted => IsExhaustedUntil.HasValue && IsExhaustedUntil > SystemTime.UtcNow;

    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    private MetricStatus()
    {
        MetricKey = null!;
    }

    internal MetricStatus(MetricKey metricKey, DateTime? isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public MetricKey MetricKey { get; }
    public DateTime? IsExhaustedUntil { get; }
}
