using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipTemplateCreated;
public class RelationshipTemplateCreatedIntegrationEvent : IntegrationEvent
{
    public required string CreatedBy { get; set; }
}
