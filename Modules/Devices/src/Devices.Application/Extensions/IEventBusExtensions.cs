using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Devices.Application.Extensions;

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
