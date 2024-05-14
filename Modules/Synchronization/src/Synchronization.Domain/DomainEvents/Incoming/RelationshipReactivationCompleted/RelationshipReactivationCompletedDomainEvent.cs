using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
public class RelationshipReactivationCompletedDomainEvent : DomainEvent
{
    public RelationshipReactivationCompletedDomainEvent(string relationshipId, string peer) : base($"{relationshipId}/Reactivation/Completed")
    {
        RelationshipId = relationshipId;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
