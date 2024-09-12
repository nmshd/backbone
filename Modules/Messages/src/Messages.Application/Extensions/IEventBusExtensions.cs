using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Messages.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Messages.Domain.DomainEvents.Incoming;

namespace Backbone.Modules.Messages.Application.Extensions;

public static class IEventBusExtensions
{
    public static IEventBus AddMessagesDomainEventSubscriptions(this IEventBus eventBus)
    {
        eventBus.Subscribe<RelationshipStatusChangedDomainEvent, RelationshipStatusChangedDomainEventHandler>();
        return eventBus;
    }
}
