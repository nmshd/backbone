using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipReactivationRequested;
public class RelationshipReactivationRequestedIntegrationEvent : IntegrationEvent
{
    public RelationshipReactivationRequestedIntegrationEvent(string relationshipId, string peer) : base($"{relationshipId}/Reactivate")
    {
        RelationshipId = relationshipId;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
