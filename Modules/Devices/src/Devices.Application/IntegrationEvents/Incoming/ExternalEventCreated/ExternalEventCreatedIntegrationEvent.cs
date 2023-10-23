using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;

public class ExternalEventCreatedIntegrationEvent : IntegrationEvent
{
    public string Owner { get; set; }
}
