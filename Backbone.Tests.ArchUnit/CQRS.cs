using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Tests.ArchUnit;
public class Cqrs
{

    private static readonly IObjectProvider<IType> COMMANDS =
        Classes().That()
            .AreNotAbstract()
            .And().AreAssignableTo(typeof(IRequest<>))
            .And().HaveName(".+Command$", true);

    private static readonly IObjectProvider<IType> QUERIES =
        Classes().That()
            .AreNotAbstract()
            .And().AreAssignableTo(typeof(IRequest<>))
            .And().HaveName(".+Query$", true);

    [Fact]
    public void ClassesInheritingFromIRequestShouldHaveNameEndingWithCommandOrQuery()
    {
        Classes()
            .That().AreAssignableTo(typeof(IRequest<>))
            .And().AreNotAbstract()
            .Should().HaveName(".+Command$", true)
            .OrShould().HaveName(".+Query$", true)
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void CommandsShouldResideInCommandsNamespace()
    {
        Classes()
            .That().Are(COMMANDS)
            .Should().ResideInNamespace(".+\\.Commands\\.", true)
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void QueriesShouldResideInQueriesNamespace()
    {
        Classes()
            .That().Are(QUERIES)
            .Should().ResideInNamespace(".+\\.Queries\\.", true)
            .Check(Backbone.ARCHITECTURE);
    }

}
