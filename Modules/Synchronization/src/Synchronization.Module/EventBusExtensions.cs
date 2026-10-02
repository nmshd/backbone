using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Files.Contracts.DomainEvents;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.FileOwnershipClaimed;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.FileOwnershipLocked;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.MessageCreated;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeleted;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerFeatureFlagsChanged;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipTemplateAllocationsExhausted;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.TokenLocked;
using Backbone.Modules.Tokens.Contracts.DomainEvents;

namespace Backbone.Modules.Synchronization.Module;

public static class IEventBusExtensions
{
    public static async Task AddSynchronizationDomainEventSubscriptions(this IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            SubscribeToMessagesEvents(eventBus),
            SubscribeToRelationshipsEvents(eventBus),
            SubscribeToTokensEvents(eventBus)
        });
    }

    private static async Task SubscribeToMessagesEvents(IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreatedDomainEventHandler>(),
            eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>(),
            eventBus.Subscribe<IdentityDeletionProcessStatusChangedDomainEvent, IdentityDeletionProcessStatusChangedDomainEventHandler>()
        });
    }

    private static async Task SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>(),
            eventBus.Subscribe<RelationshipReactivationRequestedDomainEvent, RelationshipReactivationRequestedDomainEventHandler>(),
            eventBus.Subscribe<RelationshipReactivationCompletedDomainEvent, RelationshipReactivationCompletedDomainEventHandler>(),
            eventBus.Subscribe<PeerToBeDeletedDomainEvent, PeerToBeDeletedDomainEventHandler>(),
            eventBus.Subscribe<PeerDeletionCancelledDomainEvent, PeerDeletionCancelledDomainEventHandler>(),
            eventBus.Subscribe<PeerDeletedDomainEvent, PeerDeletedDomainEventHandler>(),
            eventBus.Subscribe<PeerFeatureFlagsChangedDomainEvent, PeerFeatureFlagsChangedDomainEventHandler>(),
            eventBus.Subscribe<RelationshipTemplateAllocationsExhaustedDomainEvent, RelationshipTemplateAllocationsExhaustedDomainEventHandler>(),
            eventBus.Subscribe<FileOwnershipLockedDomainEvent, FileOwnershipLockedDomainEventHandler>(),
            eventBus.Subscribe<FileOwnershipClaimedDomainEvent, FileOwnershipClaimedDomainEventHandler>()
        });
    }

    private static async Task SubscribeToTokensEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<TokenLockedDomainEvent, TokenLockedDomainEventHandler>();
    }
}
