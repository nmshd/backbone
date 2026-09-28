using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.xUnitV3;
using Backbone.BuildingBlocks.Domain.Events;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class DomainEvents
{
    private readonly GivenTypesConjunction _moduleDomainEvents = Types()
        .That().AreAssignableTo(typeof(DomainEvent))
        .And().ResideInAssemblyMatching("Backbone.Modules.*");

    [Fact]
    public void ModuleDomainEventsShouldResideInContractAssemblies()
    {
        _moduleDomainEvents
            .Should().ResideInAssemblyMatching("Backbone.Modules.*.Contracts")
            .Because("each domain event has one canonical definition in its producer's contract assembly")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void ModuleDomainEventsShouldUseTheContractNamespace()
    {
        _moduleDomainEvents
            .Should().ResideInNamespaceMatching("\\.Contracts\\.DomainEvents$")
            .Because("domain events are shared contracts rather than module implementation details")
            .Check(Backbone.ARCHITECTURE);
    }
}
