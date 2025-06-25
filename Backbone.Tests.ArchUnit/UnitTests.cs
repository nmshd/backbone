using ArchUnitNET.xUnitV3;
using Backbone.UnitTestTools.BaseClasses;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class UnitTests
{
    [Fact]
    public void UnitTestsShouldExtendAbstractTestsBase()
    {
        Classes().That().HaveName(".+Tests$", true)
            .And().AreNot(typeof(UnitTests))
            .Should().BeAssignableTo(typeof(AbstractTestsBase))
            .Check(Backbone.ARCHITECTURE);
    }
}
