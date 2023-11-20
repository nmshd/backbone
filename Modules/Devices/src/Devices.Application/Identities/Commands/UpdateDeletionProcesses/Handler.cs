using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateDeletionProcesses;
public class Handler : IRequestHandler<UpdateDeletionProcessesCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly IEventBus _eventBus;

    public Handler(IIdentitiesRepository identitiesRepository, IEventBus eventBus)
    {
        _identitiesRepository = identitiesRepository;
        _eventBus = eventBus;
    }
    public async Task Handle(UpdateDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identitiesRepository.FindAllWithPastDeletionGracePeriod(cancellationToken, track: true);
        foreach (var identity in identities)
        {
            identity.DeletionStarted();
            await _identitiesRepository.Update(identity, cancellationToken);

            // send notification

            // create external event PeerIdentityDeleted

            // process deletion
            await _identitiesRepository.Delete(identity, cancellationToken);
            // --- delete deletion process
        }
    }
}
