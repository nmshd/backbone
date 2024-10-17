using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeleted;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeletionCancelled;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IEventBusExtensions
{
    public static void AddRelationshipsDomainEventSubscriptions(this IEventBus eventBus)
    {
        SubscribeToIdentitiesEvents(eventBus);
    }

    private static void SubscribeToIdentitiesEvents(IEventBus eventBus)
    {
        eventBus.Subscribe<IdentityToBeDeletedDomainEvent, IdentityToBeDeletedDomainEventHandler>();
        eventBus.Subscribe<IdentityDeletionCancelledDomainEvent, IdentityDeletionCancelledDomainEventHandler>();
        eventBus.Subscribe<IdentityDeletedDomainEvent, IdentityDeletedDomainEventHandler>();
    }
}
