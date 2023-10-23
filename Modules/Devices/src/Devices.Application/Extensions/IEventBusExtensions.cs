using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;
using Backbone.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;

namespace Backbone.Devices.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddDevicesIntegrationEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToSynchronizationEvents(eventBus);
    }

    private static void SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<DatawalletModifiedIntegrationEvent, DatawalletModifiedIntegrationEventHandler>();
        eventBus.Subscribe<ExternalEventCreatedIntegrationEvent, ExternalEventCreatedIntegrationEventHandler>();
    }
}
