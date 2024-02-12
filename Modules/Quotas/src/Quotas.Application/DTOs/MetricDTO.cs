using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class MetricDTO
{
    public MetricDTO(Metric metric)
    {
        Key = metric.Key.Value;
        DisplayName = metric.DisplayName;
    }

    public string Key { get; set; }
    public string DisplayName { get; set; }
}
