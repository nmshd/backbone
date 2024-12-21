using System.Collections.Concurrent;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly ConcurrentDictionary<string, List<SubscriptionInfo>> _handlers = [];

    public void Clear()
    {
        _handlers.Clear();
    }

    public void AddSubscription<T, TH>()
        where T : DomainEvent
        where TH : IDomainEventHandler<T>
    {
        DoAddSubscription(typeof(TH), typeof(T));
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
    {
        return _handlers[eventName];
    }

    public bool HasSubscriptionsForEvent<T>() where T : DomainEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        return _handlers.ContainsKey(eventName);
    }

    public string GetEventKey<T>()
    {
        return GetEventKey(typeof(T));
    }

    private static string GetEventKey(Type eventType)
    {
        return eventType.Name;
    }

    private void DoAddSubscription(Type handlerType, Type eventType)
    {
        var eventName = GetEventKey(eventType);

        _handlers.TryAdd(eventName, []);

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType, eventType));
    }

    public class SubscriptionInfo
    {
        private SubscriptionInfo(Type handlerType, Type eventType)
        {
            HandlerType = handlerType;
            EventType = eventType;
        }

        public Type HandlerType { get; }

        public Type EventType { get; }

        public static SubscriptionInfo Typed(Type handlerType, Type eventType)
        {
            return new SubscriptionInfo(handlerType, eventType);
        }
    }
}
