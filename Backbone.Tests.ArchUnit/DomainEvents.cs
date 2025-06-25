using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.xUnitV3;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Tooling.Extensions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class DomainEvents
{
    private readonly List<IType> _outgoingDomainEvents = Types()
        .That().AreAssignableTo(typeof(DomainEvent))
        .And().ResideInNamespace("\\.Outgoing", true)
        .GetObjects(Backbone.ARCHITECTURE)
        .ToList();

    private readonly GivenTypesConjunction _incomingDomainEvents = Types()
        .That().AreAssignableTo(typeof(DomainEvent))
        .And().ResideInNamespace("\\.Incoming\\.", true);

    [Fact]
    public void IncomingDomainEventsShouldNotHaveConstructors()
    {
        _incomingDomainEvents
            .Should().FollowCustomCondition(type =>
            {
                if (type.GetConstructors().Any(c => c.Parameters.Any()))
                    return new ConditionResult(type, false, "should not have a constructor with parameters");

                return new ConditionResult(type, true);
            }, "not have constructors with parameters")
            .Because(
                "objects are only created by the JSON Serializer anyway. Adding a constructor with parameters only causes errors during deserialization if the parameter names don't match the property names.")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void IncomingDomainEventsShouldHaveCorrespondingOutgoingDomainEvents()
    {
        _incomingDomainEvents
            .Should().FollowCustomCondition(type =>
            {
                var correspondingTypes = _outgoingDomainEvents.Where(t => t.NameMatches(type.Name)).ToList();
                if (correspondingTypes.IsEmpty()) return new ConditionResult(type, false, "does not have a corresponding outgoing domain event");

                foreach (var property in type.GetPropertyMembers())
                {
                    foreach (var t in correspondingTypes.Where(t => !t.HasPropertyMemberWithName(property.Name)))
                        return new ConditionResult(type, false, $"does not have a corresponding property for '{property.Name}' in its corresponding outgoing domain event ({t.FullName})");
                }

                return new ConditionResult(type, true);
            }, "have all of their properties found in their corresponding outgoing domain event")
            .Because("otherwise they would not be deserialized correctly from JSON")
            .Check(Backbone.ARCHITECTURE);
    }
}
