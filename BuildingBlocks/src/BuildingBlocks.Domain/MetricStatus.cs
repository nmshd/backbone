using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;

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

    public MetricKey MetricKey { get; private set; }
    public DateTime IsExhaustedUntil { get; private set; }
}
