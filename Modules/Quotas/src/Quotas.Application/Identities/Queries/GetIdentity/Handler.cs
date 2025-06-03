using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Metrics;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity;

public class Handler : IRequestHandler<GetIdentityQuery, GetIdentityResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IMetricsRepository _metricsRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public Handler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _identitiesRepository = identitiesRepository;
        _metricsRepository = metricsRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task<GetIdentityResponse> Handle(GetIdentityQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(request.Address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        var metricsKeys = identity.TierQuotas.Select(q => q.MetricKey).Union(identity.IndividualQuotas.Select(q => q.MetricKey));
        var metrics = await _metricsRepository.List(metricsKeys, cancellationToken);
        return await GetIdentityResponse.Create(_metricCalculatorFactory, identity.Address, identity.TierQuotas, identity.IndividualQuotas, metrics, cancellationToken);
    }
}
