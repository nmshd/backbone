using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeleted;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityDeletionCancelled;
using Backbone.Modules.Relationships.Application.DomainEvents.Incoming.IdentityToBeDeleted;
using Backbone.Modules.Relationships.Domain.DomainEvents.Incoming;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IEventBusExtensions
{
    public static async Task AddRelationshipsDomainEventSubscriptions(this IEventBus eventBus)
    {
        await SubscribeToIdentitiesEvents(eventBus);
    }

    private static async Task SubscribeToIdentitiesEvents(IEventBus eventBus)
    {
        await eventBus.Subscribe<IdentityToBeDeletedDomainEvent, IdentityToBeDeletedDomainEventHandler>();
        await eventBus.Subscribe<IdentityDeletionCancelledDomainEvent, IdentityDeletionCancelledDomainEventHandler>();
        await eventBus.Subscribe<IdentityDeletedDomainEvent, IdentityDeletedDomainEventHandler>();
    }
}
