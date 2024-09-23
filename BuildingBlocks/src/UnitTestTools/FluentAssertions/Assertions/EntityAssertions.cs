using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace FluentAssertions;
#pragma warning restore IDE0130

public class EntityAssertions : ObjectAssertions<Entity, EntityAssertions>
{
    public EntityAssertions(Entity instance) : base(instance)
    {
    }

    protected override string Identifier => "entity";

    public TEvent HaveASingleDomainEvent<TEvent>(string because = "", params object[] becauseArgs) where TEvent : DomainEvent
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.DomainEvents)
            .ForCondition(events => events.Count == 1)
            .FailWith("Expected {context:entity} to have 1 domain event, but found {0}.", Subject.DomainEvents.Count)
            .Then
            .ForCondition(events => events[0].GetType() == typeof(TEvent))
            .FailWith("Expected the domain event to be of type {0}, but found {1}.", typeof(TEvent).Name, Subject.DomainEvents[0].GetType().Name);

        return (TEvent)Subject.DomainEvents[0];
    }

    public (TEvent1 event1, TEvent2 event2) HaveDomainEvents<TEvent1, TEvent2>(string because = "", params object[] becauseArgs)
        where TEvent1 : DomainEvent
        where TEvent2 : DomainEvent
    {
        var joinedEvents = string.Join(", ", Subject.DomainEvents.Select(e => e.GetType().Name));

        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.DomainEvents)
            .ForCondition(events => events.Count == 2)
            .FailWith("Expected {context:entity} to have 2 domain events, but found {0}.", Subject.DomainEvents.Count)
            .Then
            .ForCondition(events => events.Any(e => e.GetType() == typeof(TEvent1)))
            .FailWith("Expected to find a domain event of type {0}, but only found {1}.", typeof(TEvent1).Name, joinedEvents)
            .Then
            .ForCondition(events => events.Any(e => e.GetType() == typeof(TEvent2)))
            .FailWith("Expected to find a domain event of type {0}, but only found {1}.", typeof(TEvent2).Name, joinedEvents);

        return (
            (TEvent1)Subject.DomainEvents.Single(e => e.GetType() == typeof(TEvent1)),
            (TEvent2)Subject.DomainEvents.Single(e => e.GetType() == typeof(TEvent2))
        );
    }

    public void NotHaveADomainEvent<TEvent>(string because = "", params object[] becauseArgs) where TEvent : DomainEvent
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.DomainEvents)
            .ForCondition(events => events.All(e => e is not TEvent))
            .FailWith($"Expected {{context:entity}} to not have a {typeof(TEvent).Name}, but it has.", Subject.DomainEvents);
    }
}
