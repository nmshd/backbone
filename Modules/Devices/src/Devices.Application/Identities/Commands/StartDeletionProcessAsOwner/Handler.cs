using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class Handler : IRequestHandler<StartDeletionProcessAsOwnerCommand, StartDeletionProcessAsOwnerResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
    }

    public async Task<StartDeletionProcessAsOwnerResponse> Handle(StartDeletionProcessAsOwnerCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var deletionProcess = identity.StartDeletionProcessAsOwner(_userContext.GetDeviceId());

        await _identitiesRepository.Update(identity, cancellationToken);

        return new StartDeletionProcessAsOwnerResponse(deletionProcess);
    }
}
