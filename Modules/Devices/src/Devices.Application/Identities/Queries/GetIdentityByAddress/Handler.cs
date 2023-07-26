using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetIdentityByAddress;
public class Handler : IRequestHandler<GetIdentityByAddressQuery, GetIdentityByAddressResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<GetIdentityByAddressResponse> Handle(GetIdentityByAddressQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        return new GetIdentityByAddressResponse(identity);
    }
}
