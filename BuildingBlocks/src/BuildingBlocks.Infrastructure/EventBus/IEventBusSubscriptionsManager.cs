using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public interface IEventBusSubscriptionsManager
{
    void AddSubscription<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>;

    bool HasSubscriptionsForEvent<T>() where T : DomainEvent;
    bool HasSubscriptionsForEvent(string eventName);
    void Clear();

    IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent<T>()
        where T : DomainEvent;

    IEnumerable<InMemoryEventBusSubscriptionsManager.SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}
