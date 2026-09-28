using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class RelationshipReactivationRequestedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string RequestingIdentity { get; set; }
    public required string Peer { get; set; }
}
