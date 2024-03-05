using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
public class Handler : IRequestHandler<ListMetricsQuery, ListMetricsResponse>
{
    private readonly IMetricsRepository _metricsRepository;

    public Handler(IMetricsRepository metricsRepository)
    {
        _metricsRepository = metricsRepository;
    }

    public async Task<ListMetricsResponse> Handle(ListMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _metricsRepository.FindAll(cancellationToken);
        var metricDtos = metrics.Select(m => new MetricDTO(m));

        return new ListMetricsResponse(metricDtos);
    }
}
