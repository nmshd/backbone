using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus.Events;

namespace Enmeshed.BuildingBlocks.Infrastructure.EventBus;

public partial class InMemoryEventBusSubscriptionsManager
{
    private readonly HashSet<Type> _eventTypes;
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

    public InMemoryEventBusSubscriptionsManager()
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new HashSet<Type>();
    }

    public void Clear()
    {
        _handlers.Clear();
    }

    public void AddSubscription<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        DoAddSubscription(typeof(TH), eventName);
        _eventTypes.Add(typeof(T));
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
    {
        return _handlers[eventName];
    }

    public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName)
    {
        return _handlers.ContainsKey(eventName);
    }

    public Type? GetEventTypeByName(string eventName)
    {
        return _eventTypes.SingleOrDefault(t => t.Name == eventName);
    }

    public string GetEventKey<T>()
    {
        return typeof(T).Name;
    }

    private void DoAddSubscription(Type handlerType, string eventName)
    {
        if (!HasSubscriptionsForEvent(eventName)) _handlers.Add(eventName, new List<SubscriptionInfo>());

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }
}