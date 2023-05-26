using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;

namespace Enmeshed.BuildingBlocks.Domain;
public class MetricStatus
{
    public MetricStatus(MetricKey metricKey)
    {
        MetricKey = metricKey;
    }

    private MetricKey MetricKey { get; }
    private DateTime? IsExhaustedUntil { get; }
}
