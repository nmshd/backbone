using AutoMapper;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.ListQuotaForTier;
public class Handler : IRequestHandler<ListQuotasForTierQuery, ListQuotasForTierResponse>
{
    private readonly IMapper _mapper;
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;
    private Metric _metric;
    private readonly List<TierQuotaDefinitionDTO> _response;

    public Handler(IMapper mapper, ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        _mapper = mapper;
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
        _response = new();
    }

    public async Task<ListQuotasForTierResponse> Handle(ListQuotasForTierQuery request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Find(request.TierId, cancellationToken, true);

        foreach (var item in tier.Quotas) 
        {
            _metric = await _metricsRepository.Find(item.MetricKey, cancellationToken);
            _response.Add(new TierQuotaDefinitionDTO(tier.Id,new MetricDTO(_metric.Key,_metric.DisplayName), item.Max, item.Period));
        }

        var quotaForTierDTOs = _mapper.Map<IEnumerable<TierQuotaDefinitionDTO>>(_response);

        return new ListQuotasForTierResponse(quotaForTierDTOs);
    }
}
