using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.FileOwnershipClaimed;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.FileOwnershipLocked;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerFeatureFlagsChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipTemplateAllocationsExhausted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipClaimed;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.FileOwnershipIsLocked;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFeatureFlagsChangedEvent;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipTemplateAllocationsExhausted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.TokenLocked;

namespace Backbone.Modules.Synchronization.Application.Extensions;

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
            eventBus.Subscribe<FileOwnershipLockedDomainEvent, FileOwnershipLockedEventHandler>(),
            eventBus.Subscribe<FileOwnershipClaimedDomainEvent, FileOwnershipClaimedDomainEventHandler>()
        });
    }

    private static async Task SubscribeToTokensEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<TokenLockedDomainEvent, TokenLockedDomainEventHandler>();
    }
}
