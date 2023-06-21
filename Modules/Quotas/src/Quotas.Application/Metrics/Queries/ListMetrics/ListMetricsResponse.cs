using Backbone.Modules.Quotas.Application.DTOs;
using Enmeshed.BuildingBlocks.Application.CQRS.BaseClasses;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
public class ListMetricsResponse : EnumerableResponseBase<MetricDTO>
{
    public ListMetricsResponse(IEnumerable<MetricDTO> items) : base(items) { }
}
