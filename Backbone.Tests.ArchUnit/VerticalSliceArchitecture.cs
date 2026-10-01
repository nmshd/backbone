using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.xUnitV3;
using Shouldly;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace Backbone.Backbone.Tests.ArchUnit;

public class VerticalSliceArchitecture
{
    private static readonly string[] MIGRATED_MODULES = ["Messages", "Tags"];

    [Fact]
    public void DomainsShouldNotDependOnOuterLayers()
    {
        Types().That().ResideInAssemblyMatching(@"^Backbone\.Modules\.[^.]+\.Domain(,|$)")
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching(@"^Backbone\.(Modules\..*\.(Application|Module|Infrastructure.*|Abstractions)|.*Api|BuildingBlocks\.(API|Application.*|Module|Infrastructure))(,|$)")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void ContractsAndAbstractionsShouldNotDependOnImplementations()
    {
        Types().That().ResideInAssemblyMatching(@"^Backbone\.Modules\.[^.]+\.(Contracts|Abstractions)(,|$)")
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching(@"^Backbone\.(Modules\..*\.(Application|Module|Infrastructure.*)|.*Api|BuildingBlocks\.API)(,|$)")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void InfrastructureShouldNotDependOnModulesOrApis()
    {
        Types().That().ResideInAssemblyMatching(@"^Backbone\.Modules\.[^.]+\.Infrastructure[^,]*(,|$)")
            .Should().NotDependOnAnyTypesThat().ResideInAssemblyMatching(@"^Backbone\.(Modules\.[^.]+\.Module|.*Api|BuildingBlocks\.API)(,|$)")
            .Check(Backbone.ARCHITECTURE);
    }

    [Theory]
    [InlineData("Messages")]
    [InlineData("Tags")]
    public void UseCasesShouldResideInTheirModule(string module)
    {
        Classes().That().ResideInAssemblyMatching($@"^Backbone\.Modules\.{module}\.(?!.*Tests).*$")
            .And().HaveNameMatching(".*(Command|Query|Handler|Validator)$")
            .Should().ResideInAssemblyMatching($@"^Backbone\.Modules\.{module}\.Module(,|$)")
            .Check(Backbone.ARCHITECTURE);
    }

    [Theory]
    [InlineData("Messages")]
    [InlineData("Tags")]
    public void UseCasesShouldNotDependOnAdapters(string module)
    {
        Classes().That().Are(UseCases(module)).Should().NotDependOnAnyTypesThat()
            .ResideInAssemblyMatching(@"^Backbone\.Modules\.[^.]+\.Infrastructure[^,]*(,|$)")
            .Check(Backbone.ARCHITECTURE);
        Classes().That().Are(UseCases(module)).Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching(@"Microsoft\.AspNetCore\..*")
            .Check(Backbone.ARCHITECTURE);
    }

    [Theory]
    [InlineData("Messages")]
    [InlineData("Tags")]
    public void ModulesShouldOnlyReferenceTheirOwnImplementationAndOtherContracts(string module)
    {
        foreach (var assembly in SolutionAssemblies().Where(assembly => assembly.GetName().Name == $"Backbone.Modules.{module}.Module"))
        {
            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                var name = reference.Name!;
                if (!name.StartsWith("Backbone.Modules.", StringComparison.Ordinal)) continue;
                (name.StartsWith($"Backbone.Modules.{module}.", StringComparison.Ordinal) || name.EndsWith(".Contracts", StringComparison.Ordinal) ||
                 name.EndsWith(".Abstractions", StringComparison.Ordinal)).ShouldBeTrue($"{assembly.GetName().Name} references {name}");
            }
        }
    }

    [Fact]
    public void RemovedApplicationAssembliesShouldNotBeReferencedOrPresent()
    {
        foreach (var assembly in SolutionAssemblies())
        {
            foreach (var module in MIGRATED_MODULES)
            {
                var removed = $"Backbone.Modules.{module}.Application";
                assembly.GetName().Name.ShouldNotBe(removed);
                assembly.GetReferencedAssemblies().Select(reference => reference.Name).ShouldNotContain(removed);
            }
        }
    }

    [Fact]
    public void ConsumerApiShouldReferenceMigratedModules()
    {
        var consumerApi = SolutionAssemblies().Single(assembly => assembly.GetName().Name == "Backbone.ConsumerApi");
        foreach (var module in MIGRATED_MODULES)
            consumerApi.GetReferencedAssemblies().Select(reference => reference.Name).ShouldContain($"Backbone.Modules.{module}.Module");
    }

    private static IObjectProvider<IType> UseCases(string module) =>
        Classes().That().ResideInNamespaceMatching($@"^Backbone\.Modules\.{module}\.Module\.Features\..*")
            .And().HaveNameMatching(".*(Command|Query|Handler|Validator|Response)$");

    private static IEnumerable<Assembly> SolutionAssemblies() => Directory.GetFiles(AppContext.BaseDirectory, "Backbone.*.dll")
        .Select(path => Assembly.Load(AssemblyName.GetAssemblyName(path)));
}
