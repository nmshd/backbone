using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Abstractions;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetOwnIdentity;

public class Handler : IRequestHandler<GetOwnIdentityQuery, GetOwnIdentityResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<GetOwnIdentityResponse> Handle(GetOwnIdentityQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken) ??
                       throw new Exception("Failed to retrieve identity. This could be due to an invalid JWT or data loss.");

        return new GetOwnIdentityResponse(identity);
    }
}
