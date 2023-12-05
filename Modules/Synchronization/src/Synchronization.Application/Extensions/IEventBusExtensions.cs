using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;

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
        // eventBus.Subscribe<MessageDeliveredIntegrationEvent, MessageDeliveredIntegrationEventHandler>(); // this is temporaryly disabled to avoid an external event flood when the same message is sent to many recipients (s. JSSNMSHDD-2174)
    }

    private static void SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipChangeCompletedIntegrationEvent, RelationshipChangeCompletedIntegrationEventHandler>();
        eventBus.Subscribe<RelationshipChangeCreatedIntegrationEvent, RelationshipChangeCreatedIntegrationEventHandler>();
    }
}
