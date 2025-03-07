using Backbone.BuildingBlocks.Domain.Events;

namespace Backbone.BuildingBlocks.Infrastructure.EventBus;

public static class DomainEventNamingExtensions
{
    public static string GetEventName<T>(this T @event) where T : DomainEvent
    {
        return GetEventName(@event.GetType());
    }

    public static string GetEventName(this Type eventType)
    {
#if DEBUG // we only do this when in debug mode because this method is called quite often so it might have a performance impact
        if (!eventType.IsSubclassOf(typeof(DomainEvent)))
            throw new ArgumentException("The provided type is not a domain event.");
#endif

        return eventType.Name.Replace("DomainEvent", string.Empty);
    }
}
