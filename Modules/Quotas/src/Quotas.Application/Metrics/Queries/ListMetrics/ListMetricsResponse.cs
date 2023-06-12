using Backbone.Modules.Quotas.Application.DTOs;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
public class ListMetricsResponse
{
    public IEnumerable<MetricDTO> Items { get; set; }
}
