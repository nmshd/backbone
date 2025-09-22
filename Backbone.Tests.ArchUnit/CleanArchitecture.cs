using System.Collections;
using ArchUnitNET.Domain;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class CleanArchitecture
{
    private static readonly IObjectProvider<IType> MODULES =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.*")
            .As("All Modules");

    private static readonly IObjectProvider<IType> CONSUMER_API_ASSEMBLIES =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.*.ConsumerApi")
            .As("ConsumerApi Assemblies");

    private static readonly IObjectProvider<IType> APPLICATION_ASSEMBLIES =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.*.Application")
            .As("Application Assemblies");

    private static readonly IObjectProvider<IType> INFRASTRUCTURE_ASSEMBLIES =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.*.Infrastructure")
            .As("Infrastructure Assemblies");

    [Theory]
    [ClassData(typeof(Modules))]
    public void ModulesShouldNotDependOnOtherModules(IObjectProvider<IType> module)
    {
        var otherModules = Types().That()
            .Are(MODULES)
            .And().AreNot(module)
            .As("any other module");

        Types()
            .That().Are(module)
            .And().AreNot(Backbone.TEST_TYPES)
            .And().AreNot(CONSUMER_API_ASSEMBLIES)
            .Should().NotDependOnAny(otherModules)
            .Because("modules should be self-contained.")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void ApplicationAssembliesShouldNotDependOnInfrastructureAssemblies()
    {
        Types()
            .That().Are(APPLICATION_ASSEMBLIES)
            .And().AreNot(Backbone.TEST_TYPES)
            .Should().NotDependOnAnyTypesThat().Are(INFRASTRUCTURE_ASSEMBLIES)
            .Because("this would violate Clean Architecture")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void ApplicationAssembliesShouldNotDependOnAPIAssemblies()
    {
        Types()
            .That().Are(APPLICATION_ASSEMBLIES)
            .Should().NotDependOnAnyTypesThat().ResideInAssembly("Backbone.Modules.*.API")
            .Because("this would violate Clean Architecture")
            .Check(Backbone.ARCHITECTURE);
    }

    [Fact]
    public void ApplicationAssembliesShouldNotReferenceAspNetCore()
    {
        Types()
            .That().Are(APPLICATION_ASSEMBLIES)
            .And().DoNotResideInAssemblyMatching("Backbone.Modules.Devices.Application")
            .Should().NotDependOnAnyTypesThat().ResideInNamespaceMatching("Microsoft.AspNetCore.*")
            .Because("this would violate Clean Architecture")
            .Check(Backbone.ARCHITECTURE);
    }
}

public class Modules : IEnumerable<object[]>
{
    private static readonly IObjectProvider<IType> CHALLENGES_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Challenges.*")
            .As("Challenges Module");

    private static readonly IObjectProvider<IType> DEVICES_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Devices.*")
            .As("Devices Module");

    private static readonly IObjectProvider<IType> FILES_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Files.*")
            .As("Files Module");

    private static readonly IObjectProvider<IType> MESSAGES_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Messages.*")
            .As("Messages Module");

    private static readonly IObjectProvider<IType> QUOTAS_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Quotas.*")
            .As("Quotas Module");

    private static readonly IObjectProvider<IType> RELATIONSHIPS_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Relationships.*")
            .As("Relationships Module");

    private static readonly IObjectProvider<IType> SYNCHRONIZATION_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Synchronization.*")
            .As("Synchronization Module");

    private static readonly IObjectProvider<IType> TOKENS_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Tokens.*")
            .As("Tokens Module");

    public IEnumerator<object[]> GetEnumerator()
    {
        return new List<IObjectProvider<IType>[]>
        {
            new[] { CHALLENGES_MODULE },
            new[] { DEVICES_MODULE },
            new[] { FILES_MODULE },
            new[] { MESSAGES_MODULE },
            new[] { QUOTAS_MODULE },
            new[] { RELATIONSHIPS_MODULE },
            new[] { SYNCHRONIZATION_MODULE },
            new[] { TOKENS_MODULE }
        }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
