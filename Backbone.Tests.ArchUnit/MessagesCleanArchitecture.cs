using ArchUnitNET.Domain;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class MessagesCleanArchitecture
{
    private static readonly IObjectProvider<IType> USE_CASE_TYPES =
        Classes().That()
            .ResideInNamespaceMatching("Backbone.Modules.Messages.Module.Features.*")
            .And().HaveNameMatching(".*(Command|Query|Handler|Validator|Response)$")
            .As("Messages use case types");

    [Fact]
    public void DomainShouldNotDependOnOuterLayers()
    {
        Types().That().ResideInAssemblyMatching("Backbone.Modules.Messages.Domain")
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching("Backbone.Modules.Messages.(Module|Infrastructure|Abstractions)")
            .Because("the Messages domain is the innermost Clean Architecture layer")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void AbstractionsShouldNotDependOnOuterLayers()
    {
        Types().That().ResideInAssemblyMatching("Backbone.Modules.Messages.Abstractions")
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching("Backbone.Modules.Messages.(Module|Infrastructure)")
            .Because("application ports must remain independent of adapters and composition")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void InfrastructureShouldNotDependOnModule()
    {
        Types().That().ResideInAssemblyMatching("Backbone.Modules.Messages.Infrastructure")
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching("Backbone.Modules.Messages.Module")
            .Because("infrastructure adapters may implement application ports but must not depend on the module composition root")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void UseCasesShouldNotDependOnInfrastructure()
    {
        Types().That().Are(USE_CASE_TYPES)
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching("Backbone.Modules.Messages.Infrastructure")
            .Because("use cases must depend on application ports instead of infrastructure adapters")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void UseCasesShouldNotReferenceAspNetCore()
    {
        Types().That().Are(USE_CASE_TYPES)
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching("Microsoft.AspNetCore.*")
            .Because("HTTP belongs to the endpoint adapter, not to Commands, Queries, Handlers, Validators, or Responses")
            .Check(Backbone.ARCHITECTURE);
    }
}
