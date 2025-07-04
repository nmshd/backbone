using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerToBeDeleted;

public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public PeerToBeDeletedDomainEvent(string peerOfIdentityToBeDeleted, string relationshipId, string identityToBeDeleted, DateTime gracePeriodEndsAt)
    {
        PeerOfIdentityToBeDeleted = peerOfIdentityToBeDeleted;
        RelationshipId = relationshipId;
        IdentityToBeDeleted = identityToBeDeleted;
        GracePeriodEndsAt = gracePeriodEndsAt;
    }

    public string PeerOfIdentityToBeDeleted { get; }
    public string RelationshipId { get; }
    public string IdentityToBeDeleted { get; }
    public DateTime GracePeriodEndsAt { get; }
}
