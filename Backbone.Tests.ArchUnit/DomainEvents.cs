using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.xUnit;
using Backbone.BuildingBlocks.Domain.Events;
using Backbone.Tooling.Extensions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class DomainEvents
{
    [Fact]
    public void IncomingDomainEventsShouldNotHaveConstructors()
    {
        Types()
            .That().AreAssignableTo(typeof(DomainEvent))
            .And().ResideInNamespace("Backbone.Modules.*.Domain.DomainEvents.Incoming.*", true)
            .Should().FollowCustomCondition(type =>
            {
                if (type.GetConstructors().Any(c => c.Parameters.Any()))
                    return new ConditionResult(type, false, "should not have constructors");

                return new ConditionResult(type, true);
            }, "")
            .Because("otherwise the JSON Serializer would call the constructor instead of serializing directly, which would cause an invalid serialization")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void OutgoingDomainEventsShouldHaveCorrespondingIncomingDomainEvents()
    {
        var outgoing = Types()
            .That().AreAssignableTo(typeof(DomainEvent))
            .And().ResideInNamespace("Backbone.Modules.*.DomainEvents.Outgoing.*", true)
            .GetObjects(Backbone.ARCHITECTURE)
            .ToList();

        Types()
            .That().AreAssignableTo(typeof(DomainEvent))
            .And().ResideInNamespace("Backbone.Modules.*.DomainEvents.Incoming.*", true)
            .Should().FollowCustomCondition(type =>
            {
                var correspondingTypes = outgoing.Where(t => t.NameMatches(type.Name)).ToList();
                if (correspondingTypes.IsEmpty()) return new ConditionResult(type, false, "does not have a corresponding outgoing domain event");

                foreach (var property in type.GetPropertyMembers())
                {
                    foreach (var t in correspondingTypes.Where(t => !t.HasPropertyMemberWithName(property.Name)))
                        return new ConditionResult(type, false, $"does not have a corresponding property for '{property.Name}' in its corresponding domain event ({t.FullName})");
                }

                return new ConditionResult(type, true);
            }, "")
            .Because("Reason")
            .Check(Backbone.ARCHITECTURE);
    }
}
