using Backbone.Quotas.Application.DTOs;
using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Quotas.Application.Tiers.Queries.GetTierById;
public class Handler : IRequestHandler<GetTierByIdQuery, TierDetailsDTO>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task<TierDetailsDTO> Handle(GetTierByIdQuery request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Find(request.Id, cancellationToken);

        var metricsKeys = tier.Quotas.Select(q => q.MetricKey).Distinct();
        var metrics = await _metricsRepository.FindAllWithKeys(metricsKeys, cancellationToken);

        return new TierDetailsDTO(tier, metrics);
    }
}
