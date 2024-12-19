using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.BackupDeviceUsed;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class IEventBusExtensions
{
    public static async Task AddDevicesDomainEventSubscriptions(this IEventBus eventBus)
    {
        await eventBus.SubscribeToAnnouncementsEvents();
        await eventBus.SubscribeToDevicesEvents();
        await eventBus.SubscribeToSynchronizationEvents();
    }

    private static async Task SubscribeToAnnouncementsEvents(this IEventBus eventBus)
    {
        await eventBus.Subscribe<AnnouncementCreatedDomainEvent, AnnouncementCreatedDomainEventHandler>();
    }

    private static async Task SubscribeToDevicesEvents(this IEventBus eventBus)
    {
        await eventBus.Subscribe<BackupDeviceUsedDomainEvent, BackupDeviceUsedDomainEventHandler>();
    }

    private static async Task SubscribeToSynchronizationEvents(this IEventBus eventBus)
    {
        await eventBus.Subscribe<DatawalletModifiedDomainEvent, DatawalletModifiedDomainEventHandler>();
        await eventBus.Subscribe<ExternalEventCreatedDomainEvent, ExternalEventCreatedDomainEventHandler>();
        await eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>();
    }
}
