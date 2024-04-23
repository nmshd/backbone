using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipDecomposed;
public class RelationshipDecomposedDomainEvent : DomainEvent
{
    public RelationshipDecomposedDomainEvent(string relationshipId, string peer) : base($"{relationshipId}/Decompose")
    {
        RelationshipId = relationshipId;
        Peer = peer;
    }

    public string RelationshipId { get; }
    public string Peer { get; }
}
