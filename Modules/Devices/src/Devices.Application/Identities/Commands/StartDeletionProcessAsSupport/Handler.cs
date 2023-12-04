using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;

public class Handler : IRequestHandler<StartDeletionProcessAsSupportCommand, StartDeletionProcessAsSupportResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IEventBus _eventBus;

    public Handler(IIdentitiesRepository identitiesRepository, IEventBus eventBus)
    {
        _identitiesRepository = identitiesRepository;
        _eventBus = eventBus;
    }

    public async Task<StartDeletionProcessAsSupportResponse> Handle(StartDeletionProcessAsSupportCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.IdentityAddress, cancellationToken, true) ?? throw new NotFoundException(nameof(Identity));
        var deletionProcess = identity.StartDeletionProcessAsSupport();

        await _identitiesRepository.Update(identity, cancellationToken);

        _eventBus.Publish(new IdentityDeletionProcessStartedIntegrationEvent(identity, deletionProcess));

        return new StartDeletionProcessAsSupportResponse(deletionProcess);
    }
}
