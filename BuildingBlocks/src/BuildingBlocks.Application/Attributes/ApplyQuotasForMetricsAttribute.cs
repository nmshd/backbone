using Enmeshed.BuildingBlocks.Domain.StronglyTypedIds;

namespace Enmeshed.BuildingBlocks.Application.Attributes;
public class ApplyQuotasForMetricsAttribute : Attribute
{
    private MetricKey[] MetricKeys { get; }

    public ApplyQuotasForMetricsAttribute(params string[] metricKeys)
    {
        MetricKeys = metricKeys.Select(it=> new MetricKey(it)).ToArray();
    }
}
