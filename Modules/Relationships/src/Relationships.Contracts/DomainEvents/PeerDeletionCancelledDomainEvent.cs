using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class PeerDeletionCancelledDomainEvent : DomainEvent
{
    public required string PeerOfIdentityWithDeletionCancelled { get; set; }
    public required string RelationshipId { get; set; }
    public required string IdentityWithDeletionCancelled { get; set; }
}
