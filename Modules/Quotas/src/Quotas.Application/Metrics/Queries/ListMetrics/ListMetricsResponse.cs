using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;

public class ListMetricsResponse : CollectionResponseBase<MetricDTO>
{
    public ListMetricsResponse(IEnumerable<Metric> items) : base(items.Select(m => new MetricDTO(m)))
    {
    }
}
