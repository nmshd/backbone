using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.RelationshipReactivationRequested;
public class RelationshipReactivationRequestedIntegrationEvent : IntegrationEvent
{
    public required string CreatedBy { get; set; }
}
