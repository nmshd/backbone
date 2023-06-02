using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class MetricDTO
{
    public MetricDTO(MetricKey key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public MetricKey Key { get; set; }
    public string DisplayName { get; set; }
}
