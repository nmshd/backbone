using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class RelationshipReactivationCompletedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string NewRelationshipStatus { get; set; }
    public required string Peer { get; set; }
}
