using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class RelationshipTemplateAllocationsExhaustedDomainEvent : DomainEvent
{
    public required string RelationshipTemplateId { get; set; }
    public required string CreatedBy { get; set; }
}
