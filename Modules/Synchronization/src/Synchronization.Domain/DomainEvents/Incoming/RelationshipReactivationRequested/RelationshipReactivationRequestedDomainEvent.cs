using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationRequested;

public class RelationshipReactivationRequestedDomainEvent : DomainEvent
{
    public RelationshipReactivationRequestedDomainEvent(string relationshipId, string peer) : base($"{relationshipId}/Reactivate")
    {
        RelationshipId = relationshipId;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
