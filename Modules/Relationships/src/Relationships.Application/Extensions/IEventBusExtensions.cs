using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class IEventBusExtensions
{
    public static IEventBus AddRelationshipsIntegrationEventSubscriptions(this IEventBus eventBus)
    {
        return eventBus;
    }
}
