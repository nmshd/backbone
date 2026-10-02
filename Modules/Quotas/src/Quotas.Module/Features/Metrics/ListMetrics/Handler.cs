using Backbone.Modules.Quotas.Abstractions;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.ListMetrics;

public class Handler : IRequestHandler<Query, Response>
{
    private readonly IMetricsRepository _metricsRepository;

    public Handler(IMetricsRepository metricsRepository)
    {
        _metricsRepository = metricsRepository;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var metrics = await _metricsRepository.List(cancellationToken);
        return new Response(metrics);
    }
}
