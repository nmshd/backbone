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
        var metrics = await _metricsRepository.List(cancellationToken);
        return new ListMetricsResponse(metrics);
    }
}
