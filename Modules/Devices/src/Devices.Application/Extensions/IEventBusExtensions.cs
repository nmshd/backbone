using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;

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
        eventBus.Subscribe<IdentityDeletionProcessStartedIntegrationEvent, IdentityDeletionProcessStartedIntegrationEventHandler>();
    }
}
