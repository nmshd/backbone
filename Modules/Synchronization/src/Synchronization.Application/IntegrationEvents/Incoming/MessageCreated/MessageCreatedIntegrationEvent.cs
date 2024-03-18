using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;

public class MessageCreatedIntegrationEvent : IntegrationEvent
{
    public required string Id { get; set; }
    public required IEnumerable<string> Recipients { get; set; }
}
