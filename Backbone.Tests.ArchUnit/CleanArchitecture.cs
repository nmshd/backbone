using System.Collections;
using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Tests.ArchUnit;

public class CleanArchitecture
{
    private static readonly IObjectProvider<IType> Modules =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*", true)
            .As("All Modules");

    private static readonly IObjectProvider<IType> ApplicationAssemblies =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*.Application", true)
            .As("Application Assemblies");

    private static readonly IObjectProvider<IType> InfrastructureAssemblies =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*.Infrastructure", true)
            .As("Infrastructure Assemblies");

    [Theory]
    [ClassData(typeof(Modules))]
    public void ModulesShouldNotDependOnOtherModules(IObjectProvider<IType> module)
    {
        var otherModules = Types().That()
            .Are(Modules)
            .And().AreNot(module)
            .As("any other module");

        Types()
            .That().Are(module)
            .Should().NotDependOnAny(otherModules)
            .Because("modules should be self-contained.")
            .Check(Backbone.Architecture);
    }

    [Fact]
    public void ApplicationAssembliesShouldNotDependOnInfrastructureAssemblies()
    {
        Types()
            .That().Are(ApplicationAssemblies)
            .Should().NotDependOnAnyTypesThat().Are(InfrastructureAssemblies)
            .Because("this would violate Clean Architecture")
            .Check(Backbone.Architecture);
    }

    [Fact]
    public void ApplicationAssembliesShouldNotDependOnAPIAssemblies()
    {
        Types()
            .That().Are(ApplicationAssemblies)
            .Should().NotDependOnAnyTypesThat().ResideInAssembly("Backbone.Modules.*.API")
            .Because("this would violate Clean Architecture")
            .Check(Backbone.Architecture);
    }

    [Fact]
    public void ApplicationAssembliesShouldNotReferenceAspNetCore()
    {
        Types()
            .That().Are(ApplicationAssemblies)
            .Should().NotDependOnAnyTypesThat().ResideInNamespace("Microsoft.AspNetCore.*", true)
            .Because("this would violate Clean Architecture")
            .Check(Backbone.Architecture);
    }
}

public class Modules : IEnumerable<object[]>
{
    private static readonly IObjectProvider<IType> ChallengesModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Challenges.*", true)
            .As("Challenges Module");

    private static readonly IObjectProvider<IType> DevicesModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Devices.*", true)
            .As("Devices Module");

    private static readonly IObjectProvider<IType> FilesModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Files.*", true)
            .As("Files Module");

    private static readonly IObjectProvider<IType> MessagesModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Messages.*", true)
            .As("Messages Module");

    private static readonly IObjectProvider<IType> RelationshipsModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Relationships.*", true)
            .As("Relationships Module");

    private static readonly IObjectProvider<IType> SynchronizationModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Synchronization.*", true)
            .As("Synchronization Module");

    private static readonly IObjectProvider<IType> TokensModule =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Tokens.*", true)
            .As("Tokens Module");

    public IEnumerator<object[]> GetEnumerator()
    {
        return new List<IObjectProvider<IType>[]>
        {
            new[] { ChallengesModule },
            new[] { DevicesModule },
            new[] { FilesModule },
            new[] { MessagesModule },
            new[] { RelationshipsModule },
            new[] { SynchronizationModule },
            new[] { TokensModule }
        }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}