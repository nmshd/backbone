using System.Collections;
using ArchUnitNET.Domain;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Backbone.Backbone.Tests.ArchUnit;

public class CleanArchitecture
{
    private static readonly IObjectProvider<IType> MODULES =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*", true)
            .As("All Modules");

    private static readonly IObjectProvider<IType> CONSUMER_API_ASSEMBLIES =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*.ConsumerApi", true)
            .As("ConsumerApi Assemblies");

    private static readonly IObjectProvider<IType> APPLICATION_ASSEMBLIES =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*.Application", true)
            .As("Application Assemblies");

    private static readonly IObjectProvider<IType> INFRASTRUCTURE_ASSEMBLIES =
        Types().That()
            .ResideInAssembly("Backbone.Modules.*.Infrastructure", true)
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
            .And().DoNotResideInAssembly("Backbone.Modules.Devices.Application", true)
            .Should().NotDependOnAnyTypesThat().ResideInNamespace("Microsoft.AspNetCore.*", true)
            .Because("this would violate Clean Architecture")
            .Check(Backbone.ARCHITECTURE);
    }
}

public class Modules : IEnumerable<object[]>
{
    private static readonly IObjectProvider<IType> CHALLENGES_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Challenges.*", true)
            .As("Challenges Module");

    private static readonly IObjectProvider<IType> DEVICES_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Devices.*", true)
            .As("Devices Module");

    private static readonly IObjectProvider<IType> FILES_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Files.*", true)
            .As("Files Module");

    private static readonly IObjectProvider<IType> MESSAGES_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Messages.*", true)
            .As("Messages Module");

    private static readonly IObjectProvider<IType> QUOTAS_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Quotas.*", true)
            .As("Quotas Module");

    private static readonly IObjectProvider<IType> RELATIONSHIPS_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Relationships.*", true)
            .As("Relationships Module");

    private static readonly IObjectProvider<IType> SYNCHRONIZATION_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Synchronization.*", true)
            .As("Synchronization Module");

    private static readonly IObjectProvider<IType> TOKENS_MODULE =
        Types().That()
            .ResideInAssembly("Backbone.Modules.Tokens.*", true)
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
