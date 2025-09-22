using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class RelationshipTemplateAllocationsExhaustedDomainEvent : DomainEvent
{
    public RelationshipTemplateAllocationsExhaustedDomainEvent(RelationshipTemplate template) : base($"{template.Id}/maxNumberOfAllocationsExhausted")
    {
        RelationshipTemplateId = template.Id;
        CreatedBy = template.CreatedBy;
    }

    public string RelationshipTemplateId { get; set; }
    public string CreatedBy { get; set; }
}
