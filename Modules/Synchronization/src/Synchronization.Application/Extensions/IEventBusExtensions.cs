using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationRequested;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.TokenLocked;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class IEventBusExtensions
{
    public static async Task AddSynchronizationDomainEventSubscriptions(this IEventBus eventBus)
    {
        await SubscribeToMessagesEvents(eventBus);
        await SubscribeToRelationshipsEvents(eventBus);
        await SubscribeToTokensEvents(eventBus);
    }

    private static async Task SubscribeToMessagesEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreatedDomainEventHandler>();
        await eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>();
        await eventBus.Subscribe<IdentityDeletionProcessStatusChangedDomainEvent, IdentityDeletionProcessStatusChangedDomainEventHandler>();
    }

    private static async Task SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
        await eventBus.Subscribe<RelationshipReactivationRequestedDomainEvent, RelationshipReactivationRequestedDomainEventHandler>();
        await eventBus.Subscribe<RelationshipReactivationCompletedDomainEvent, RelationshipReactivationCompletedDomainEventHandler>();
        await eventBus.Subscribe<PeerToBeDeletedDomainEvent, PeerToBeDeletedDomainEventHandler>();
        await eventBus.Subscribe<PeerDeletionCancelledDomainEvent, PeerDeletionCancelledDomainEventHandler>();
        await eventBus.Subscribe<PeerDeletedDomainEvent, PeerDeletedDomainEventHandler>();
    }

    private static async Task SubscribeToTokensEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<TokenLockedDomainEvent, TokenLockedDomainEventHandler>();
    }
}
