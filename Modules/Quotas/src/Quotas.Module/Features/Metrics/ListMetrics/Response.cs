using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Module.Features.Shared;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.ListMetrics;

public class ListMetricsResponse : CollectionResponseBase<MetricDTO>
{
    public ListMetricsResponse(IEnumerable<Metric> items) : base(items.Select(m => new MetricDTO(m)))
    {
    }
}
