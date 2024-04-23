using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class IEventBusExtensions
{
    public static IEventBus AddSynchronizationDomainEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToMessagesEvents(eventBus);
        SubscribeToRelationshipsEvents(eventBus);

        return eventBus;
    }

    private static void SubscribeToMessagesEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<MessageCreatedDomainEvent, MessageCreatedDomainEventHandler>();
        eventBus.Subscribe<IdentityDeletionProcessStartedDomainEvent, IdentityDeletionProcessStartedDomainEventHandler>();
        eventBus.Subscribe<IdentityDeletionProcessStatusChangedDomainEvent, IdentityDeletionProcessStatusChangedDomainEventHandler>();
        // eventBus.Subscribe<MessageDeliveredDomainEvent, MessageDeliveredDomainEventHandler>(); // this is temporarily disabled to avoid an external event flood when the same message is sent to many recipients
    }

    private static void SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipCreatedDomainEvent, RelationshipCreatedDomainEventHandler>();
        eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
    }
}
