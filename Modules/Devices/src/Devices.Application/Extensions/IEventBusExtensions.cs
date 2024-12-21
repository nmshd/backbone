using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.BackupDeviceUsed;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddDevicesDomainEventSubscriptions(this IEventBus eventBus)
    {
        eventBus.SubscribeToAnnouncementsEvents();
        eventBus.SubscribeToDevicesEvents();
        eventBus.SubscribeToSynchronizationEvents();
        eventBus.SubscribeToTokensEvents();
    }

    private static void SubscribeToAnnouncementsEvents(this IEventBus eventBus)
    {
        eventBus.Subscribe<AnnouncementCreatedDomainEvent, AnnouncementCreatedDomainEventHandler>();
    }

    private static void SubscribeToDevicesEvents(this IEventBus eventBus)
    {
        eventBus.Subscribe<BackupDeviceUsedDomainEvent, BackupDeviceUsedDomainEventHandler>();
    }

    private static void SubscribeToSynchronizationEvents(this IEventBus eventBus)
    {
        eventBus.Subscribe<DatawalletModifiedDomainEvent, DatawalletModifiedDomainEventHandler>();
        eventBus.Subscribe<ExternalEventCreatedDomainEvent, ExternalEventCreatedDomainEventHandler>();
        eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>();
    }

    private static void SubscribeToTokensEvents(this IEventBus eventBus)
    {
        eventBus.Subscribe<TokenLockedDomainEvent, TokenLockedDomainEventHandler>();
    }
}
