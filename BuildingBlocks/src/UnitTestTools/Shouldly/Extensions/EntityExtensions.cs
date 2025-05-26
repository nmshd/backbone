using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.UnitTestTools.Shouldly.Messages;
using Shouldly;

namespace Backbone.UnitTestTools.Shouldly.Extensions;

[ShouldlyMethods]
public static class EntityExtensions
{
    public static TEvent ShouldHaveASingleDomainEvent<TEvent>(this Entity instance, string? customMessage = null) where TEvent : DomainEvent
    {
        if (instance.DomainEvents.Count != 1)
            throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent), instance.DomainEvents.Count, customMessage).ToString());

        var domainEvent = instance.DomainEvents[0];

        if (domainEvent is TEvent) return (TEvent)instance.DomainEvents[0];

        throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent), domainEvent.GetType(), customMessage).ToString());
    }

    public static (TEvent1 event1, TEvent2 event2) ShouldHaveDomainEvents<TEvent1, TEvent2>(this Entity entity, string? customMessage = null) where TEvent1 : DomainEvent where TEvent2 : DomainEvent
    {
        if (entity.DomainEvents.Count != 2)
            throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent1), entity.DomainEvents.Count, customMessage).ToString());

        if (entity.DomainEvents.All(e => e.GetType() != typeof(TEvent1)))
            throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent1), entity.DomainEvents.Select(e => e.GetType()), customMessage).ToString());

        if (entity.DomainEvents.All(e => e.GetType() != typeof(TEvent2)))
            throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent2), entity.DomainEvents.Select(e => e.GetType()), customMessage).ToString());

        return (
            (TEvent1)entity.DomainEvents.Single(e => e.GetType() == typeof(TEvent1)),
            (TEvent2)entity.DomainEvents.Single(e => e.GetType() == typeof(TEvent2))
        );
    }

    public static void ShouldNotHaveADomainEvent<TEvent>(this Entity entity, string? customMessage = null) where TEvent : DomainEvent
    {
        if (entity.DomainEvents.Any(e => e.GetType() == typeof(TEvent)))
            throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent), customMessage).ToString());
    }
}
