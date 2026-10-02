using AnnouncementCreated = Backbone.Modules.Devices.Module.Features.DomainEvents.AnnouncementCreated;
using BackupDeviceUsed = Backbone.Modules.Devices.Module.Features.DomainEvents.BackupDeviceUsed;
using DatawalletModificationCreated = Backbone.Modules.Devices.Module.Features.DomainEvents.DatawalletModificationCreated;
using ExternalEventCreated = Backbone.Modules.Devices.Module.Features.DomainEvents.ExternalEventCreated;
using IdentityDeletionProcessStarted = Backbone.Modules.Devices.Module.Features.DomainEvents.IdentityDeletionProcessStarted;
using TokenLocked = Backbone.Modules.Devices.Module.Features.DomainEvents.TokenLocked;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Contracts.DomainEvents;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;
using Backbone.Modules.Tokens.Contracts.DomainEvents;

namespace Backbone.Modules.Devices.Module;

internal static class DevicesEventBusExtensions
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
            await eventBus.Subscribe<AnnouncementCreatedDomainEvent, AnnouncementCreated.Handler>();
        }

        private async Task SubscribeToDevicesEvents()
        {
            await eventBus.Subscribe<BackupDeviceUsedDomainEvent, BackupDeviceUsed.Handler>();
        }

        private async Task SubscribeToSynchronizationEvents()
        {
            await Task.WhenAll(new List<Task>
            {
                eventBus.Subscribe<DatawalletModifiedDomainEvent, DatawalletModificationCreated.Handler>(),
                eventBus.Subscribe<ExternalEventCreatedDomainEvent, ExternalEventCreated.Handler>(),
                eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStarted.Handler>()
            });
        }

        private async Task SubscribeToTokensEvents()
        {
            await eventBus.Subscribe<TokenLockedDomainEvent, TokenLocked.Handler>();
        }
    }
}
