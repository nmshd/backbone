using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Relationships.Application.IntegrationEvents.Incoming;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IEventBusExtensions
{
    public static IEventBus AddRelationshipsIntegrationEventSubscriptions(this IEventBus eventBus)
    {
        eventBus.Subscribe<IdentityToBeDeletedIntegrationEvent, IdentityToBeDeletedIntegrationEventHandler>();
        return eventBus;
    }
}
