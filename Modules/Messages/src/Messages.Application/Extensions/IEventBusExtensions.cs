using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Application.DomainEvents.Incoming.MessageOrphaned;
using Backbone.Modules.Messages.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Messages.Domain.DomainEvents.Incoming;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddMessagesDomainEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToMessagesEvents(eventBus);
        SubscribeToRelationshipsEvents(eventBus);
    }

    private static void SubscribeToMessagesEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<MessageOrphanedDomainEvent, MessageOrphanedDomainEventHandler>();
    }

    private static void SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
    }
}
