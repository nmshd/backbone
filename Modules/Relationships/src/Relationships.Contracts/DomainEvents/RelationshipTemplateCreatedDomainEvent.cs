using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.Modules.Relationships.Contracts.DomainEvents;

public class RelationshipTemplateCreatedDomainEvent : DomainEvent
{
    public required string TemplateId { get; set; }
    public required string CreatedBy { get; set; }
}
