using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTier;

public class Handler : IRequestHandler<GetTierQuery, TierDetailsDTO>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task<TierDetailsDTO> Handle(GetTierQuery request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Get(request.Id, cancellationToken) ?? throw new NotFoundException(nameof(Tier));

        var metricsKeys = tier.Quotas.Select(q => q.MetricKey).Distinct();
        var metrics = await _metricsRepository.List(metricsKeys, cancellationToken);

        return new TierDetailsDTO(tier, metrics);
    }
}
