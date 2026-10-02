using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetIdentity;

public class Handler : IRequestHandler<GetIdentityQuery, GetIdentityResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<GetIdentityResponse> Handle(GetIdentityQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(request.Address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        return new GetIdentityResponse(identity);
    }
}
