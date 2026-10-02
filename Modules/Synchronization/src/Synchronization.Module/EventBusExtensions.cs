using FileOwnershipClaimed = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.FileOwnershipClaimed;
using FileOwnershipLocked = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.FileOwnershipLocked;
using IdentityDeletionProcessStarted = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStarted;
using IdentityDeletionProcessStatusChanged = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStatusChanged;
using MessageCreated = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.MessageCreated;
using PeerDeleted = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeleted;
using PeerDeletionCancelled = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeletionCancelled;
using PeerFeatureFlagsChanged = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerFeatureFlagsChanged;
using PeerToBeDeleted = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerToBeDeleted;
using RelationshipReactivationCompleted = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipReactivationCompleted;
using RelationshipReactivationRequested = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipReactivationRequested;
using RelationshipStatusChanged = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipStatusChanged;
using RelationshipTemplateAllocationsExhausted = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.RelationshipTemplateAllocationsExhausted;
using TokenLocked = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.TokenLocked;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Files.Contracts.DomainEvents;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
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
            eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreated.Handler>(),
            eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStarted.Handler>(),
            eventBus.Subscribe<IdentityDeletionProcessStatusChangedDomainEvent, IdentityDeletionProcessStatusChanged.Handler>()
        });
    }

    private static async Task SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChanged.Handler>(),
            eventBus.Subscribe<RelationshipReactivationRequestedDomainEvent, RelationshipReactivationRequested.Handler>(),
            eventBus.Subscribe<RelationshipReactivationCompletedDomainEvent, RelationshipReactivationCompleted.Handler>(),
            eventBus.Subscribe<PeerToBeDeletedDomainEvent, PeerToBeDeleted.Handler>(),
            eventBus.Subscribe<PeerDeletionCancelledDomainEvent, PeerDeletionCancelled.Handler>(),
            eventBus.Subscribe<PeerDeletedDomainEvent, PeerDeleted.Handler>(),
            eventBus.Subscribe<PeerFeatureFlagsChangedDomainEvent, PeerFeatureFlagsChanged.Handler>(),
            eventBus.Subscribe<RelationshipTemplateAllocationsExhaustedDomainEvent, RelationshipTemplateAllocationsExhausted.Handler>(),
            eventBus.Subscribe<FileOwnershipLockedDomainEvent, FileOwnershipLocked.Handler>(),
            eventBus.Subscribe<FileOwnershipClaimedDomainEvent, FileOwnershipClaimed.Handler>()
        });
    }

    private static async Task SubscribeToTokensEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<TokenLockedDomainEvent, TokenLocked.Handler>();
    }
}
