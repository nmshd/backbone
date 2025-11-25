using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.UnitTestTools.Shouldly.Messages;
using Shouldly;

namespace Backbone.UnitTestTools.Shouldly.Extensions;

[ShouldlyMethods]
public static class EntityExtensions
{
    extension(Entity instance)
    {
        public TEvent ShouldHaveASingleDomainEvent<TEvent>(string? customMessage = null) where TEvent : DomainEvent
        {
            if (instance.DomainEvents.Count != 1)
                throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent), instance.DomainEvents.Count, customMessage).ToString());

            var domainEvent = instance.DomainEvents[0];

            if (domainEvent is TEvent) return (TEvent)instance.DomainEvents[0];

            throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent), domainEvent.GetType(), customMessage).ToString());
        }

        public (TEvent1 event1, TEvent2 event2) ShouldHaveDomainEvents<TEvent1, TEvent2>(string? customMessage = null) where TEvent1 : DomainEvent where TEvent2 : DomainEvent
        {
            if (instance.DomainEvents.Count != 2)
                throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent1), instance.DomainEvents.Count, customMessage).ToString());

            if (instance.DomainEvents.All(e => e.GetType() != typeof(TEvent1)))
                throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent1), instance.DomainEvents.Select(e => e.GetType()), customMessage).ToString());

            if (instance.DomainEvents.All(e => e.GetType() != typeof(TEvent2)))
                throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent2), instance.DomainEvents.Select(e => e.GetType()), customMessage).ToString());

            return (
                (TEvent1)instance.DomainEvents.Single(e => e.GetType() == typeof(TEvent1)),
                (TEvent2)instance.DomainEvents.Single(e => e.GetType() == typeof(TEvent2))
            );
        }

        public void ShouldNotHaveADomainEvent<TEvent>(string? customMessage = null) where TEvent : DomainEvent
        {
            if (instance.DomainEvents.Any(e => e.GetType() == typeof(TEvent)))
                throw new ShouldAssertException(new DomainEventShouldlyMessage(typeof(TEvent), customMessage).ToString());
        }
    }
}
