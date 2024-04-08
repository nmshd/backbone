using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public interface IEventBusSubscriptionsManager
{
    void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>;

    bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
    bool HasSubscriptionsForEvent(string eventName);
    void Clear();

    IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent<T>()
        where T : IntegrationEvent;

    IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}
