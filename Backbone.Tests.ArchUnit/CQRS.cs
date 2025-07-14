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
    public void ClassesInheritingFromIRequestShouldHaveNameEndingWithCommandOrQuery()
    {
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .Should().HaveNameMatching(".+(Command|Query)$").As("should have names ending with 'Command' or 'Query'")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void CommandsShouldResideInCommandsNamespace()
    {
        Classes().That().Are(COMMANDS)
            .Should().ResideInNamespaceMatching(".+\\.Commands\\.")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void QueriesShouldResideInQueriesNamespace()
    {
        Classes().That().Are(QUERIES)
            .Should().ResideInNamespaceMatching(".+\\.Queries\\.")
            .Check(Backbone.ARCHITECTURE);
    }
}
