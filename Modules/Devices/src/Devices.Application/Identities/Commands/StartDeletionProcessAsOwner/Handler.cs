using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;

public class Handler : IRequestHandler<StartDeletionProcessAsOwnerCommand, StartDeletionProcessAsOwnerResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IUserContext _userContext;
    private readonly IEventBus _eventBus;

    public Handler(IIdentitiesRepository identitiesRepository, IUserContext userContext, IEventBus eventBus)
    {
        _identitiesRepository = identitiesRepository;
        _userContext = userContext;
        _eventBus = eventBus;
    }

    public async Task<StartDeletionProcessAsOwnerResponse> Handle(StartDeletionProcessAsOwnerCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var oldTierId = identity.TierId;
        var deletionProcess = identity.StartDeletionProcessAsOwner(_userContext.GetDeviceId());
        var newTierId = identity.TierId;

        _eventBus.Publish(new TierOfIdentityChangedIntegrationEvent(identity, oldTierId!, newTierId!));

        await _identitiesRepository.Update(identity, cancellationToken);

        return new StartDeletionProcessAsOwnerResponse(deletionProcess);
    }
}
