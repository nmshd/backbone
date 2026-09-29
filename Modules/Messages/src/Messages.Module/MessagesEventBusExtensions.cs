using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Contracts.DomainEvents;
using Backbone.Modules.Messages.Module.Features.DomainEvents.MessageOrphaned;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using RelationshipStatusChangedHandler = Backbone.Modules.Messages.Module.Features.DomainEvents.RelationshipStatusChanged.Handler;

namespace Backbone.Modules.Messages.Module;

internal static class MessagesEventBusExtensions
{
    public static async Task AddMessagesDomainEventSubscriptions(this IEventBus eventBus)
    {
        await Task.WhenAll(
            eventBus.Subscribe<MessageOrphanedDomainEvent, Handler>(),
            eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedHandler>());
    }
}
