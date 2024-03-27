using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents.Outgoing;

public class RelationshipTemplateCreatedIntegrationEvent : IntegrationEvent
{
    public RelationshipTemplateCreatedIntegrationEvent(RelationshipTemplate template) : base($"{template.Id}/Created")
    {
        TemplateId = template.Id;
        CreatedBy = template.CreatedBy;
    }

    public string TemplateId { get; private set; }
    public string CreatedBy { get; private set; }
}
