using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Synchronization.Application.IntegrationEvents.Incoming.MessageCreated;
using Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCompleted;
using Synchronization.Application.IntegrationEvents.Incoming.RelationshipChangeCreated;

namespace Synchronization.Application.Extensions;

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
        // eventBus.Subscribe<MessageDeliveredIntegrationEvent, MessageDeliveredIntegrationEventHandler>(); // this is temporaryly disabled to avoid an external event flood when the same message is sent to many recipients (s. JSSNMSHDD-2174)
    }

    private static void SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipChangeCompletedIntegrationEvent, RelationshipChangeCompletedIntegrationEventHandler>();
        eventBus.Subscribe<RelationshipChangeCreatedIntegrationEvent, RelationshipChangeCreatedIntegrationEventHandler>();
    }
}
