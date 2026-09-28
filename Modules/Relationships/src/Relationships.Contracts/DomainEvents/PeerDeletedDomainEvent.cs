using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class PeerDeletedDomainEvent : DomainEvent
{
    public required string PeerOfDeletedIdentity { get; set; }
    public required string RelationshipId { get; set; }
    public required string DeletedIdentity { get; set; }
}
