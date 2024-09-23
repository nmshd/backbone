using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerDeletionCancelledDomainEvent : DomainEvent
{
    public PeerDeletionCancelledDomainEvent(IdentityAddress peerOfIdentityWithDeletionCancelled, RelationshipId relationshipId, IdentityAddress identityWithDeletionCancelled)
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
