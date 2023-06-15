using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class MetricDTO : IMapTo<Metric>
{
    public MetricDTO(MetricKey key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public MetricKey Key { get; set; }
    public string DisplayName { get; set; }
}
