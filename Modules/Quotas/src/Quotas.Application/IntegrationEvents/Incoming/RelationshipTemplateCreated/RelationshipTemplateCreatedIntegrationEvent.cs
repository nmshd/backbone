using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;
public class RelationshipTemplateCreatedIntegrationEvent : IntegrationEvent
{
    public string CreatedBy { get; set; }
}
