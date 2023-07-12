using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Queries.GetTierById;
public class Handler : IRequestHandler<GetTierByIdQuery, GetTierByIdResponse>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task<GetTierByIdResponse> Handle(GetTierByIdQuery request, CancellationToken cancellationToken)
    {
        var tier = await _tiersRepository.Find(request.Id, cancellationToken);

        var metricsKeys = tier.Quotas.Select(q => q.MetricKey).Distinct();
        var metrics = await _metricsRepository.FindAllWithKeys(metricsKeys, cancellationToken);

        return new GetTierByIdResponse(tier, metrics);
    }
}