using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerDeletionCancelledDomainEvent : DomainEvent
{
    public PeerDeletionCancelledDomainEvent(string peerOfIdentityWithDeletionCanceled, string relationshipId, string identityWithDeletionCanceled)
        : base($"{relationshipId}/peerDeletionCanceled/{identityWithDeletionCanceled}", randomizeId: true)
    {
        PeerOfIdentityWithDeletionCanceled = peerOfIdentityWithDeletionCanceled;
        RelationshipId = relationshipId;
        IdentityWithDeletionCanceled = identityWithDeletionCanceled;
    }

    public string PeerOfIdentityWithDeletionCanceled { get; }
    public string RelationshipId { get; }
    public string IdentityWithDeletionCanceled { get; }
}
