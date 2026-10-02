using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Relationships.Module.Features.DomainEvents.FeatureFlagsOfIdentityChanged;
using Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeleted;
using Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeletionCancelled;
using Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityToBeDeleted;

namespace Backbone.Modules.Relationships.Module;

internal static class RelationshipsEventBusExtensions
{
    public static async Task AddRelationshipsDomainEventSubscriptions(this IEventBus eventBus)
    {
        await SubscribeToIdentitiesEvents(eventBus);
    }

    private static async Task SubscribeToIdentitiesEvents(IEventBus eventBus)
    {
        await Task.WhenAll(new List<Task>
        {
            eventBus.Subscribe<IdentityToBeDeletedDomainEvent, IdentityToBeDeletedDomainEventHandler>(),
            eventBus.Subscribe<IdentityDeletionCancelledDomainEvent, IdentityDeletionCancelledDomainEventHandler>(),
            eventBus.Subscribe<IdentityDeletedDomainEvent, IdentityDeletedDomainEventHandler>(),
            eventBus.Subscribe<FeatureFlagsOfIdentityChangedDomainEvent, FeatureFlagsOfIdentityChangedDomainEventHandler>()
        });
    }
}
