using Backbone.Modules.Relationships.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Relationships.Application.IntegrationEvents;
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
