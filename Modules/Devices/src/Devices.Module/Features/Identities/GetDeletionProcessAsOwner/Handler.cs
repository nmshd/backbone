using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsOwner;

public class Handler : IRequestHandler<Query, IdentityDeletionProcessOverviewDTO>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<IdentityDeletionProcessOverviewDTO> Handle(Query request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.Get(_userContext.GetAddress(), cancellationToken) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.DeletionProcesses.FirstOrDefault(p => p.Id == request.Id) ?? throw new NotFoundException(nameof(IdentityDeletionProcess));
        var response = new IdentityDeletionProcessOverviewDTO(deletionProcess);

        return response;
    }
}
