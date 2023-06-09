using Backbone.Modules.Quotas.Application.DTOs;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
public class ListMetricsQuery : IRequest<IEnumerable<MetricDTO>>
{
}
