using Backbone.BuildingBlocks.Domain;

namespace Backbone.BuildingBlocks.Application.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ApplyQuotasForMetricsAttribute : Attribute
{
    protected MetricKey[] MetricKeys { get; }

    public ApplyQuotasForMetricsAttribute(params string[] metricKeys)
    {
        MetricKeys = metricKeys.Select(it => new MetricKey(it)).ToArray();
    }
}
