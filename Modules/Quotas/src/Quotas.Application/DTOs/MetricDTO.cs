using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Quotas.Application.DTOs;

public class MetricDTO : IMapTo<Metric>
{
    public MetricDTO() {}

    public MetricDTO(Metric metric)
    {
        Key = metric.Key.Value;
        DisplayName = metric.DisplayName;
    }

    public string Key { get; set; }
    public string DisplayName { get; set; }
}
