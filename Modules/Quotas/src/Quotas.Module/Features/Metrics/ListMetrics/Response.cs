using Backbone.BuildingBlocks.Application.CQRS.BaseClasses;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Module.Features.Shared;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.ListMetrics;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListMetricsResponse")]
public class Response : CollectionResponseBase<MetricDTO>
{
    public Response(IEnumerable<Metric> items) : base(items.Select(m => new MetricDTO(m)))
    {
    }
}
