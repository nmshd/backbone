using AutoMapper;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Metrics.Queries.ListMetrics;
public class Handler : IRequestHandler<ListMetricsQuery, ListMetricsResponse>
{
    private readonly IMapper _mapper;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(IMapper mapper, IMetricsRepository metricsRepository)
    {
        _mapper = mapper;
        _metricsRepository = metricsRepository;
    }

    public async Task<ListMetricsResponse> Handle(ListMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _metricsRepository.FindAll(cancellationToken);
        var metricDtos = metrics.Select(m => new MetricDTO(m));

        return new ListMetricsResponse(metricDtos);
    }
}
