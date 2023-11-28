using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class Handler : IRequestHandler<StartDeletionProcessAsSupportCommand, StartDeletionProcessAsSupportResponse>
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

    public async Task<StartDeletionProcessAsSupportResponse> Handle(StartDeletionProcessAsSupportCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.StartDeletionProcessBySupport(_userContext.GetDeviceId());

        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new IdentityDeletionProcessStartedIntegrationEvent(identity, deletionProcess));

        return new StartDeletionProcessAsSupportResponse(deletionProcess);
    }
}
