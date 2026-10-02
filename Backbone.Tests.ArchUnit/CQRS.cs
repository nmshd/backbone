using ArchUnitNET.Domain;
using ArchUnitNET.xUnitV3;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class Cqrs
{
    private static readonly IObjectProvider<IType> NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST =
        Classes()
            .That().AreAssignableTo(typeof(IRequest<>)).Or().AreAssignableTo(typeof(IRequest)).As("Classes that implement 'IRequest'")
            .And().AreNotAbstract();

    private static readonly IObjectProvider<IType> COMMANDS =
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .And().AreNot(Backbone.TEST_TYPES)
            .And().HaveNameEndingWith("Command");

    private static readonly IObjectProvider<IType> QUERIES =
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .And().HaveNameEndingWith("Query");

    [Fact]
    public void ClassesInheritingFromIRequestShouldBeNamedCommandOrQuery()
    {
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .And().AreNot(Backbone.TEST_TYPES)
            .Should().HaveNameMatching("^(Command|Query)$").As("should be named 'Command' or 'Query'")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void CommandsShouldResideInFeaturesNamespace()
    {
        Classes().That().Are(COMMANDS)
            .Should().ResideInNamespaceMatching(@"^Backbone\.Modules\.[^.]+\.Module\.Features\..*")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void QueriesShouldResideInFeaturesNamespace()
    {
        Classes().That().Are(QUERIES)
            .Should().ResideInNamespaceMatching(@"^Backbone\.Modules\.[^.]+\.Module\.Features\..*")
            .Check(Backbone.ARCHITECTURE);
    }
}
