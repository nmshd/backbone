using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public PeerToBeDeletedDomainEvent(IdentityAddress peerOfIdentityToBeDeleted, RelationshipId relationshipId, IdentityAddress identityToBeDeleted, DateTime gracePeriodEndsAt)
        : base($"{relationshipId}/peerToBeDeleted/{identityToBeDeleted}", randomizeId: true)
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
