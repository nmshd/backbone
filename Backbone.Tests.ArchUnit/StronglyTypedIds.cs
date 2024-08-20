using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.xUnit;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class StronglyTypedIds
{
    [Fact]
    public void StronglyTypedIdsShouldHaveIsValidMethod()
    {
        Classes().That().AreAssignableTo(typeof(StronglyTypedId))
            .And().AreNot(typeof(StronglyTypedId))
            .Should().FollowCustomCondition((type) =>
            {
                var methods = type.GetMethodMembers();

                return methods.Any(c => c.NameContains("IsValid") && c.Parameters.Any(p => p.Name == nameof(String)))
                    ? new ConditionResult(type, true)
                    : new ConditionResult(type, false, "Entity should have 'IsValid' method");
            }, "")
            .Check(Backbone.ARCHITECTURE);
    }
}
