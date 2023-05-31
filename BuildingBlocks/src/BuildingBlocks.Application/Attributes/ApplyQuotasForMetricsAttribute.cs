using Enmeshed.BuildingBlocks.Domain;

namespace Enmeshed.BuildingBlocks.Application.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ApplyQuotasForMetricsAttribute : Attribute
{
    private MetricKey[] MetricKeys { get; }

    public ApplyQuotasForMetricsAttribute(params string[] metricKeys)
    {
        MetricKeys = metricKeys.Select(it=> new MetricKey(it)).ToArray();
    }
}
