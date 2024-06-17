using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerDeletedDomainEvent : DomainEvent
{
    public PeerDeletedDomainEvent(string peerOfDeletedIdentity, string relationshipId, string deletedIdentity)
        : base($"{relationshipId}/peerDeletionCancelled/{deletedIdentity}")
    {
        PeerOfDeletedIdentity = peerOfDeletedIdentity;
        RelationshipId = relationshipId;
        DeletedIdentity = deletedIdentity;
    }

    public string PeerOfDeletedIdentity { get; }
    public string RelationshipId { get; }
    public string DeletedIdentity { get; }
}
