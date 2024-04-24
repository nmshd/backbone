using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddDevicesDomainEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToSynchronizationEvents(eventBus);
    }

    private static void SubscribeToSynchronizationEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<DatawalletModifiedDomainEvent, DatawalletModifiedDomainEventHandler>();
        eventBus.Subscribe<ExternalEventCreatedDomainEvent, ExternalEventCreatedDomainEventHandler>();
        eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>();
    }
}
