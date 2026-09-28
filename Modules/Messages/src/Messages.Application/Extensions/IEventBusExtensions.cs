using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Messages.Application.DomainEvents.Incoming.MessageOrphaned;
using Backbone.Modules.Messages.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Relationships.Contracts.DomainEvents;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class IEventBusExtensions
{
    public static async Task AddMessagesDomainEventSubscriptions(this IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            SubscribeToMessagesEvents(eventBus),
            SubscribeToRelationshipsEvents(eventBus)
        });
    }

    private static async Task SubscribeToMessagesEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<MessageOrphanedDomainEvent, MessageOrphanedDomainEventHandler>();
    }

    private static async Task SubscribeToRelationshipsEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
    }
}
