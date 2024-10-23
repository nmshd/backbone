using Backbone.BuildingBlocks.Domain.Events;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class PeerDeletedDomainEvent : DomainEvent
{
    public PeerDeletedDomainEvent(IdentityAddress peerOfDeletedIdentity, RelationshipId relationshipId, IdentityAddress deletedIdentity)
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
