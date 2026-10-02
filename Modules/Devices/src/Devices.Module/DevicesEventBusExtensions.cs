using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Announcements.Contracts.DomainEvents;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Devices.Module.Features.DomainEvents.AnnouncementCreated;
using Backbone.Modules.Devices.Module.Features.DomainEvents.BackupDeviceUsed;
using Backbone.Modules.Devices.Module.Features.DomainEvents.DatawalletModificationCreated;
using Backbone.Modules.Devices.Module.Features.DomainEvents.ExternalEventCreated;
using Backbone.Modules.Devices.Module.Features.DomainEvents.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Module.Features.DomainEvents.TokenLocked;
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
