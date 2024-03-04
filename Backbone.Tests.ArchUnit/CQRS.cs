using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;
public class Cqrs
{
    private static readonly IObjectProvider<IType> NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST =
            Classes()
                .That().AreAssignableTo(typeof(IRequest<>)).Or().AreAssignableTo(typeof(IRequest)).As("Classes that implement 'IRequest'")
                .And().AreNotAbstract()
                .And().HaveName(".+Command$", true);

    private static readonly IObjectProvider<IType> COMMANDS =
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .And().HaveName(".+Command$", true);

    private static readonly IObjectProvider<IType> QUERIES =
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .And().HaveName(".+Query$", true);

    [Fact]
    public void ClassesInheritingFromIRequestShouldHaveNameEndingWithCommandOrQuery()
    {
        Classes().That().Are(NON_ABSTRACT_CLASSES_IMPLEMENTING_IREQUEST)
            .Should().HaveName(".+(Command|Query)$", true).As("should have names ending with 'Command' or 'Query'")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void CommandsShouldResideInCommandsNamespace()
    {
        Classes().That().Are(COMMANDS)
            .Should().ResideInNamespace(".+\\.Commands\\.", true)
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void QueriesShouldResideInQueriesNamespace()
    {
        Classes().That().Are(QUERIES)
            .Should().ResideInNamespace(".+\\.Queries\\.", true)
            .Check(Backbone.ARCHITECTURE);
    }

}
