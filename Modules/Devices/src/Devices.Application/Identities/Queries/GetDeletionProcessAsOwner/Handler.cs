using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsOwner;

public class Handler : IRequestHandler<GetDeletionProcessAsOwnerQuery, IdentityDeletionProcessOverviewDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<IdentityDeletionProcessOverviewDTO> Handle(GetDeletionProcessAsOwnerQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(p => p.Id == request.Id) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
        var response = new IdentityDeletionProcessOverviewDTO(deletionProcess);

        return response;
    }
}
