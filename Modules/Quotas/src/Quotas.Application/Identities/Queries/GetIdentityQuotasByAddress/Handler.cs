﻿using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentityQuotasByAddress;
public class Handler : IRequestHandler<GetIdentityQuotasByAddressQuery, GetIdentityQuotasByAddressResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(IIdentitiesRepository identitiesRepository, IMetricsRepository metricsRepository)
    {
        _identitiesRepository = identitiesRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task<GetIdentityQuotasByAddressResponse> Handle(GetIdentityQuotasByAddressQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Find(request.Address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        var metricsKeys = identity.TierQuotas.Select(q => q.MetricKey).Union(identity.IndividualQuotas.Select(q => q.MetricKey));
        var metrics = await _metricsRepository.FindAllWithKeys(metricsKeys, cancellationToken);

        return new GetIdentityQuotasByAddressResponse(identity.Address, identity.TierQuotas, identity.IndividualQuotas, metrics);
    }
}
