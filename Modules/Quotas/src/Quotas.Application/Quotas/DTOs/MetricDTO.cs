using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Mapping;

namespace Backbone.Modules.Quotas.Application.Quotas.DTOs;

public class MetricDTO : IMapTo<Metric>
{
    public MetricKey Key { get; }
    public string DisplayName { get; }
}
