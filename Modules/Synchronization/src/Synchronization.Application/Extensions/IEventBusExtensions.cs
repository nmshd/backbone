using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipChangeCreated;

namespace Backbone.Modules.Synchronization.Application.Extensions;

public static class IEventBusExtensions
{
    public static IEventBus AddSynchronizationIntegrationEventSubscriptions(this IEventBus eventBus)
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
        // eventBus.Subscribe<MessageDeliveredIntegrationEvent, MessageDeliveredIntegrationEventHandler>(); // this is temporarily disabled to avoid an external event flood when the same message is sent to many recipients
    }

    private static void SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipChangeCompletedDomainEvent, RelationshipChangeCompletedDomainEventHandler>();
        eventBus.Subscribe<RelationshipChangeCreatedDomainEvent, RelationshipChangeCreatedDomainEventHandler>();
    }
}
