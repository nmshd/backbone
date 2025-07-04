using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCancelled;

public class PeerDeletionCancelledDomainEvent : DomainEvent
{
    public PeerDeletionCancelledDomainEvent(string peerOfIdentityWithDeletionCancelled, string relationshipId, string identityWithDeletionCancelled)
    {
        PeerOfIdentityWithDeletionCancelled = peerOfIdentityWithDeletionCancelled;
        RelationshipId = relationshipId;
        IdentityWithDeletionCancelled = identityWithDeletionCancelled;
    }

    public string PeerOfIdentityWithDeletionCancelled { get; }
    public string RelationshipId { get; }
    public string IdentityWithDeletionCancelled { get; }
}
