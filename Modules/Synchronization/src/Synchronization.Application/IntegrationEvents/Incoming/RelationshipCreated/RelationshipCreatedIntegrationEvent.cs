using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipCreated;

public class RelationshipCreatedIntegrationEvent : IntegrationEvent
{
    public required string RelationshipId { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
}
