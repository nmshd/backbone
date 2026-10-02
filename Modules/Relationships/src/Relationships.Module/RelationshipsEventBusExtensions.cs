using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Devices.Contracts.DomainEvents;
using FeatureFlagsOfIdentityChanged = Backbone.Modules.Relationships.Module.Features.DomainEvents.FeatureFlagsOfIdentityChanged;
using IdentityDeleted = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeleted;
using IdentityDeletionCancelled = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityDeletionCancelled;
using IdentityToBeDeleted = Backbone.Modules.Relationships.Module.Features.DomainEvents.IdentityToBeDeleted;

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
            eventBus.Subscribe<IdentityToBeDeletedDomainEvent, IdentityToBeDeleted.Handler>(),
            eventBus.Subscribe<IdentityDeletionCancelledDomainEvent, IdentityDeletionCancelled.Handler>(),
            eventBus.Subscribe<IdentityDeletedDomainEvent, IdentityDeleted.Handler>(),
            eventBus.Subscribe<FeatureFlagsOfIdentityChangedDomainEvent, FeatureFlagsOfIdentityChanged.Handler>()
        });
    }
}
