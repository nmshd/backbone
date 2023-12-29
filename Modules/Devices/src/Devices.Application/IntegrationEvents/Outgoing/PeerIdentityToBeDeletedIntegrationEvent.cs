using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

public class PeerIdentityToBeDeletedIntegrationEvent : IntegrationEvent
{
    public PeerIdentityToBeDeletedIntegrationEvent(string identityAddress, string deletionProcessId) : base($"{identityAddress}/IdentityToBeDeleted/{deletionProcessId}")
    {
        DeletionProcessId = deletionProcessId;
        Address = identityAddress;
    }

    public string Address { get; private set; }
    public string DeletionProcessId { get; private set; }
}
