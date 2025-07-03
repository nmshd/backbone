using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Domain.DomainEvents.Outgoing;

public class RelationshipTemplateCreatedDomainEvent : DomainEvent
{
    public RelationshipTemplateCreatedDomainEvent(RelationshipTemplate template) : base($"{template.Id}/Created")
    {
        TemplateId = template.Id;
        CreatedBy = template.CreatedBy;
    }

    public string TemplateId { get; init; }
    public string CreatedBy { get; init; }
}
