using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using MediatR;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Tests.ArchUnit;
public class CQRS
{

    private static readonly IObjectProvider<IType> Commands =
        Classes().That()
            .AreNotAbstract()
            .And().AreAssignableTo(typeof(IRequest<>))
            .And().HaveName(".+Command$", true);

    private static readonly IObjectProvider<IType> Queries =
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
            .Check(Backbone.Architecture);
    }

    [Fact]
    public void CommandsShouldResideInCommandsNamespace()
    {
        Classes()
            .That().Are(Commands)
            .Should().ResideInNamespace(".+\\.Commands\\.", true)
            .Check(Backbone.Architecture);
    }

    [Fact]
    public void QueriesShouldResideInQueriesNamespace()
    {
        Classes()
            .That().Are(Queries)
            .Should().ResideInNamespace(".+\\.Queries\\.", true)
            .Check(Backbone.Architecture);
    }

}
