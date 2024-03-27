using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipStatusChanged;

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
        eventBus.Subscribe<MessageCreatedIntegrationEvent, MessageCreatedIntegrationEventHandler>();
        eventBus.Subscribe<IdentityDeletionProcessStartedIntegrationEvent, IdentityDeletionProcessStartedIntegrationEventHandler>();
        eventBus.Subscribe<IdentityDeletionProcessStatusChangedIntegrationEvent, IdentityDeletionProcessStatusChangedIntegrationEventHandler>();
        // eventBus.Subscribe<MessageDeliveredIntegrationEvent, MessageDeliveredIntegrationEventHandler>(); // this is temporarily disabled to avoid an external event flood when the same message is sent to many recipients
    }

    private static void SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipCreatedIntegrationEvent, RelationshipCreatedIntegrationEventHandler>();
        eventBus.Subscribe<RelationshipStatusChangedIntegrationEvent, RelationshipStatusChangedIntegrationEventHandler>();
    }
}
