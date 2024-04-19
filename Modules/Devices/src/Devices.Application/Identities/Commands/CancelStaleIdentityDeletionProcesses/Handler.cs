using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CancelStaleIdentityDeletionProcesses;

public class Handler : IRequestHandler<CancelStaleIdentityDeletionProcessesCommand, CancelStaleIdentityDeletionProcessesResponse>
{
    private readonly IEventBus _eventBus;
    private readonly IIdentitiesRepository _identityRepository;

    public Handler(IIdentitiesRepository identityRepository, IEventBus eventBus)
    {
        _identityRepository = identityRepository;
        _eventBus = eventBus;
    }

    public async Task<CancelStaleIdentityDeletionProcessesResponse> Handle(CancelStaleIdentityDeletionProcessesCommand request, CancellationToken cancellationToken)
    {
        var identities = await _identityRepository.FindAllWithDeletionProcessInStatus(DeletionProcessStatus.WaitingForApproval, cancellationToken, track: true);

        var idsOfCancelledDeletionProcesses = new List<IdentityDeletionProcessId>();

        foreach (var identity in identities)
        {
            var deletionProcess = identity.CancelStaleDeletionProcess();

            if (deletionProcess.IsFailure)
                continue;

            idsOfCancelledDeletionProcesses.Add(deletionProcess.Value.Id);

            await _identityRepository.Update(identity, cancellationToken);

            _eventBus.Publish(new IdentityDeletionProcessStatusChangedDomainEvent(identity.Address, deletionProcess.Value.Id));
        }

        return new CancelStaleIdentityDeletionProcessesResponse(idsOfCancelledDeletionProcesses);
    }
}
