using Backbone.BuildingBlocks.Domain;

namespace Backbone.BuildingBlocks.Application.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ApplyQuotasForMetricsAttribute : Attribute
{
    public ApplyQuotasForMetricsAttribute(params string[] metricKeys)
    {
        MetricKeys = metricKeys.Select(it => new MetricKey(it)).ToArray();
    }

    public MetricKey[] MetricKeys { get; }
}
