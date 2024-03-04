using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
public class ListMetricsResponse : CollectionResponseBase<MetricDTO>
{
    public ListMetricsResponse(IEnumerable<MetricDTO> items) : base(items) { }
}
