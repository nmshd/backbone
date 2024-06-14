using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Events;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Backbone.UnitTestTools.FluentAssertions.Assertions;

public class EntityAssertions : ReferenceTypeAssertions<Entity, EntityAssertions>
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
            .FailWith("Expected {context:entity} to have 1 domain event, but found {0}.",
                Subject.DomainEvents.Count)
            .Then
            .ForCondition(events => events[0].GetType() == typeof(TEvent))
            .FailWith("Expected the domain event to be of type {0}, but found {1}.",
                typeof(TEvent), Subject.DomainEvents[0].GetType());

        return (TEvent)Subject.DomainEvents[0];
    }

    public (TEvent1, TEvent2) HaveDomainEvent<TEvent1, TEvent2>(string because = "", params object[] becauseArgs)
        where TEvent1 : DomainEvent
        where TEvent2 : DomainEvent
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.DomainEvents)
            .ForCondition(events => events.Count > 1)
            .FailWith("Expected {context:entity} to have at least 2 domain events.")
            .Then
            .ForCondition(events => events[0].GetType() == typeof(TEvent1) || events[1].GetType() == typeof(TEvent1))
            .FailWith("Expected domain events to contain type {0}, but did not found it.",
                typeof(TEvent1))
            .Then
            .ForCondition(events => events[0].GetType() == typeof(TEvent2) || events[1].GetType() == typeof(TEvent2))
            .FailWith("Expected domain events to contain type {0}, but did not found it.",
                typeof(TEvent2));

        return ((TEvent1)Subject.DomainEvents.Single(e => e.GetType() == typeof(TEvent1)),
            (TEvent2)Subject.DomainEvents.Single(e => e.GetType() == typeof(TEvent2)));
    }
}
