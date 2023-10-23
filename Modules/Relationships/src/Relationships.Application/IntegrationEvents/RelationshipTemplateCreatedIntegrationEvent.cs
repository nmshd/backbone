using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;
using Backbone.Relationships.Domain.Entities;

namespace Backbone.Relationships.Application.IntegrationEvents;
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
