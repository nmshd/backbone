using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.xUnitV3;
using Backbone.BuildingBlocks.Domain;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class DomainDrivenDesign
{
    [Fact]
    public void EntitiesShouldHaveEmptyDefaultConstructors()
    {
        Types()
            .That().AreAssignableTo(typeof(Entity))
            .Should().FollowCustomCondition(type =>
            {
                var constructors = type.GetConstructors();

                if (constructors.All(c => c.Parameters.Any()))
                    return new ConditionResult(type, false, "Entity should have a parameterless constructor");

                return new ConditionResult(type, true);
            }, "")
            .Because("otherwise the real constructor would be called by EF Core, which would add a domain event to the collection")
            .Check(Backbone.ARCHITECTURE);
    }
}
