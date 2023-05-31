namespace Enmeshed.BuildingBlocks.Domain;
public class MetricStatus
{
    public MetricStatus(MetricKey metricKey)
    {
        MetricKey = metricKey;
    }

    public MetricStatus(MetricKey metricKey, DateTime isExhaustedUntil)
    {
        MetricKey = metricKey;
        IsExhaustedUntil = isExhaustedUntil;
    }

    public MetricKey MetricKey { get; }
    public DateTime IsExhaustedUntil { get; }
}
