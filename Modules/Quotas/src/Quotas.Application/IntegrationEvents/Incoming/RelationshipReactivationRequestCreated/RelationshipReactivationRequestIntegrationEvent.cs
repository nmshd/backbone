using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipReactivationRequestCreated;
public class RelationshipReactivationRequestIntegrationEvent : IntegrationEvent
{
    public required string CreatedBy { get; set; }
}
