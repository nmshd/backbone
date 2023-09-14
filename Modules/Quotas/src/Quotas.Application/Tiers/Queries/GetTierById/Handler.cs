using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class Handler : IRequestHandler<GetTierByIdQuery, TierDetailsDTO>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository, IIdentitiesRepository identityRepository)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
        _identityRepository = identityRepository;
    }

    public async Task<TierDetailsDTO> Handle(GetTierByIdQuery request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Find(request.Id, cancellationToken);

        var metricsKeys = tier.Quotas.Select(q => q.MetricKey).Distinct();
        var metrics = await _metricsRepository.FindAllWithKeys(metricsKeys, cancellationToken);

        var identities = await _identityRepository.FindWithTier(new TierId(request.Id), cancellationToken);
        var identitiesCount = identities.Count();

        return new TierDetailsDTO(tier, metrics, identitiesCount);
    }
}
