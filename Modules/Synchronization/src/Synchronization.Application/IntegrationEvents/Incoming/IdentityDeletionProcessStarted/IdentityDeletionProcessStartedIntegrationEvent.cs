using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;

public class IdentityDeletionProcessStartedIntegrationEvent : IntegrationEvent
{
    public IdentityDeletionProcessStartedIntegrationEvent(string identityAddress, string deletionProcessId) : base($"{identityAddress}/DeletionProcessStarted/{deletionProcessId}")
    {
        DeletionProcessId = deletionProcessId;
        Address = identityAddress;
    }

    public string Address { get; private set; }
    public string DeletionProcessId { get; private set; }
}
