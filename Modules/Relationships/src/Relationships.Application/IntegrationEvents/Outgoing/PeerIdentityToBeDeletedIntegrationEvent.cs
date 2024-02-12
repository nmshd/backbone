using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;

public class PeerIdentityToBeDeletedIntegrationEvent : IntegrationEvent
{
    public PeerIdentityToBeDeletedIntegrationEvent(string identityAddress, string deletionProcessId, string relationshipId) : base($"{identityAddress}/PeerIdentityToBeDeleted/{deletionProcessId}/Relationship/{relationshipId}")
    {
        Address = identityAddress;
    }

    public string Address { get; }
}
