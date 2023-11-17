using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.Entities;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;

public class Handler : IRequestHandler<StartDeletionProcessCommand, StartDeletionProcessResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IEventBus _eventBus;
    private readonly IUserContext _userContext;

    public Handler(IIdentitiesRepository identitiesRepository, IEventBus eventBus, IUserContext userContext)
    {
        _identitiesRepository = identitiesRepository;
        _eventBus = eventBus;
        _userContext = userContext;
    }

    public async Task<StartDeletionProcessResponse> Handle(StartDeletionProcessCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));

        var deletionProcess = identity.StartDeletionProcess(_userContext.GetDeviceId());

        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new IdentityDeletionProcessStartedIntegrationEvent(identity.Address));
        
        return new StartDeletionProcessResponse(deletionProcess);
    }
}
