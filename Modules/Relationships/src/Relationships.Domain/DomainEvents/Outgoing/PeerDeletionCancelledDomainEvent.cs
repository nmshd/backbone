using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerDeletionCancelledDomainEvent : DomainEvent
{
    public PeerDeletionCancelledDomainEvent(string peerOfIdentityWithDeletionCancelled, string relationshipId, string identityWithDeletionCancelled)
        : base($"{relationshipId}/peerDeletionCancelled/{identityWithDeletionCancelled}", randomizeId: true)
    {
        PeerOfIdentityWithDeletionCancelled = peerOfIdentityWithDeletionCancelled;
        RelationshipId = relationshipId;
        IdentityWithDeletionCancelled = identityWithDeletionCancelled;
    }

    public string PeerOfIdentityWithDeletionCancelled { get; }
    public string RelationshipId { get; }
    public string IdentityWithDeletionCancelled { get; }
}
