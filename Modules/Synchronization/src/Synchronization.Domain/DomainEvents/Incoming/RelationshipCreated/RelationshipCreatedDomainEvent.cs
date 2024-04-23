using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipCreated;

public class RelationshipCreatedDomainEvent : DomainEvent
{
    public required string RelationshipId { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
}
