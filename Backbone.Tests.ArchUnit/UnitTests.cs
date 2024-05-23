using ArchUnitNET.xUnit;
using Backbone.UnitTestTools.BaseClasses;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class UnitTests
{
    [Fact]
    public void QueriesShouldResideInQueriesNamespace()
    {
        Classes().That().HaveName(".+Tests$", true)
            .And().AreNot(typeof(UnitTests))
            .Should().BeAssignableTo(typeof(AbstractTestsBase))
            .Check(Backbone.ARCHITECTURE);
    }
}
