using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipTemplateAllocationsExhausted;

public class RelationshipTemplateAllocationsExhaustedDomainEvent : DomainEvent
{
    public required string RelationshipTemplateId { get; set; }
    public required string CreatedBy { get; set; }
}
