using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Quotas.Application.DTOs;

namespace Backbone.Quotas.Application.Metrics.Queries.ListMetrics;
public class ListMetricsResponse : EnumerableResponseBase<MetricDTO>
{
    public ListMetricsResponse(IEnumerable<MetricDTO> items) : base(items) { }
}
