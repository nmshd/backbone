using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.BackupDeviceUsed;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Announcements.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;
using Backbone.Modules.Tokens.Contracts.DomainEvents;
using Backbone.Modules.Devices.Contracts.DomainEvents;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class IEventBusExtensions
{
    extension(IEventBus eventBus)
    {
        public async Task AddDevicesDomainEventSubscriptions()
        {
            await Task.WhenAll(new List<Task>
            {
                eventBus.SubscribeToAnnouncementsEvents(),
                eventBus.SubscribeToDevicesEvents(),
                eventBus.SubscribeToSynchronizationEvents(),
                eventBus.SubscribeToTokensEvents()
            });
        }

        private async Task SubscribeToAnnouncementsEvents()
        {
            await eventBus.Subscribe<AnnouncementCreatedDomainEvent, AnnouncementCreatedDomainEventHandler>();
        }

        private async Task SubscribeToDevicesEvents()
        {
            await eventBus.Subscribe<BackupDeviceUsedDomainEvent, BackupDeviceUsedDomainEventHandler>();
        }

        private async Task SubscribeToSynchronizationEvents()
        {
            await Task.WhenAll(new List<Task>
            {
                eventBus.Subscribe<DatawalletModifiedDomainEvent, DatawalletModifiedDomainEventHandler>(),
                eventBus.Subscribe<ExternalEventCreatedDomainEvent, ExternalEventCreatedDomainEventHandler>(),
                eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>()
            });
        }

        private async Task SubscribeToTokensEvents()
        {
            await eventBus.Subscribe<TokenLockedDomainEvent, TokenLockedDomainEventHandler>();
        }
    }
}
