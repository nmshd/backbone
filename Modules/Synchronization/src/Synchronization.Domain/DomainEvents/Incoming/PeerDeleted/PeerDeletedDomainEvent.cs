using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;

public class PeerDeletedDomainEvent : DomainEvent
{
    public PeerDeletedDomainEvent(string peerOfDeletedIdentity, string relationshipId, string deletedIdentity)
    {
        PeerOfDeletedIdentity = peerOfDeletedIdentity;
        RelationshipId = relationshipId;
        DeletedIdentity = deletedIdentity;
    }

    public string PeerOfDeletedIdentity { get; }
    public string RelationshipId { get; }
    public string DeletedIdentity { get; }
}
