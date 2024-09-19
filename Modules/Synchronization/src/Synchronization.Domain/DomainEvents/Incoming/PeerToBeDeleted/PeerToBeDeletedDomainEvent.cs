using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerToBeDeleted;

public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public PeerToBeDeletedDomainEvent(string peerOfIdentityToBeDeleted, string relationshipId, string identityToBeDeleted)
    {
        PeerOfIdentityToBeDeleted = peerOfIdentityToBeDeleted;
        RelationshipId = relationshipId;
        IdentityToBeDeleted = identityToBeDeleted;
    }

    public string PeerOfIdentityToBeDeleted { get; }
    public string RelationshipId { get; }
    public string IdentityToBeDeleted { get; }
}
