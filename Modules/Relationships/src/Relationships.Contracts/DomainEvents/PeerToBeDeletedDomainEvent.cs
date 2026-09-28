using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class PeerToBeDeletedDomainEvent : DomainEvent
{
    public required string PeerOfIdentityToBeDeleted { get; set; }
    public required string RelationshipId { get; set; }
    public required string IdentityToBeDeleted { get; set; }
    public required DateTime GracePeriodEndsAt { get; set; }
}
