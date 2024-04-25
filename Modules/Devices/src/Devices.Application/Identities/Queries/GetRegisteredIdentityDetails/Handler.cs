using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetRegisteredIdentityDetails;
public class Handler : IRequestHandler<GetRegisteredIdentityDetailsQuery, GetRegisteredIdentityDetailsResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<GetRegisteredIdentityDetailsResponse> Handle(GetRegisteredIdentityDetailsQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        return new GetRegisteredIdentityDetailsResponse(identity);
    }
}
