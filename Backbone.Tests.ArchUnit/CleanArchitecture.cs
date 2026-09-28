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

    private static readonly IObjectProvider<IType> CONTRACT_ASSEMBLIES =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.*.Contracts")
            .As("Contract Assemblies");

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
            .And().AreNot(CONTRACT_ASSEMBLIES)
            .As("any other module");

        Types()
            .That().Are(module)
            .And().AreNot(Backbone.TEST_TYPES)
            .And().AreNot(CONSUMER_API_ASSEMBLIES)
            .Should().NotDependOnAny(otherModules)
            .Because("modules should be self-contained.")
            .Check(Backbone.ARCHITECTURE);
    }

    [Theory]
    [ClassData(typeof(Contracts))]
    public void ContractAssembliesShouldNotDependOnModules(IObjectProvider<IType> contractAssembly)
    {
        var moduleTypesOutsideContract = Types().That()
            .Are(MODULES)
            .And().AreNot(contractAssembly)
            .As("module types outside the contract assembly");

        Types()
            .That().Are(contractAssembly)
            .Should().NotDependOnAny(moduleTypesOutsideContract)
            .Because("contracts must not depend on module-specific projects.")
            .WithoutRequiringPositiveResults()
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
    private static readonly IObjectProvider<IType> ANNOUNCEMENTS_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Announcements.*")
            .As("Announcements Module");

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

    private static readonly IObjectProvider<IType> TAGS_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Tags.*")
            .As("Tags Module");

    private static readonly IObjectProvider<IType> TOKENS_MODULE =
        Types().That()
            .ResideInAssemblyMatching("Backbone.Modules.Tokens.*")
            .As("Tokens Module");

    public IEnumerator<object[]> GetEnumerator()
    {
        return new List<IObjectProvider<IType>[]>
        {
            new[] { ANNOUNCEMENTS_MODULE },
            new[] { CHALLENGES_MODULE },
            new[] { DEVICES_MODULE },
            new[] { FILES_MODULE },
            new[] { MESSAGES_MODULE },
            new[] { QUOTAS_MODULE },
            new[] { RELATIONSHIPS_MODULE },
            new[] { SYNCHRONIZATION_MODULE },
            new[] { TAGS_MODULE },
            new[] { TOKENS_MODULE }
        }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class Contracts : IEnumerable<object[]>
{
    private static readonly IObjectProvider<IType> ANNOUNCEMENTS_CONTRACTS = ContractAssembly("Announcements");
    private static readonly IObjectProvider<IType> DEVICES_CONTRACTS = ContractAssembly("Devices");
    private static readonly IObjectProvider<IType> FILES_CONTRACTS = ContractAssembly("Files");
    private static readonly IObjectProvider<IType> MESSAGES_CONTRACTS =
        ContractAssembly("Messages");
    private static readonly IObjectProvider<IType> QUOTAS_CONTRACTS = ContractAssembly("Quotas");
    private static readonly IObjectProvider<IType> RELATIONSHIPS_CONTRACTS = ContractAssembly("Relationships");
    private static readonly IObjectProvider<IType> SYNCHRONIZATION_CONTRACTS = ContractAssembly("Synchronization");
    private static readonly IObjectProvider<IType> TOKENS_CONTRACTS = ContractAssembly("Tokens");

    public IEnumerator<object[]> GetEnumerator()
    {
        return new List<IObjectProvider<IType>[]>
        {
            new[] { ANNOUNCEMENTS_CONTRACTS },
            new[] { DEVICES_CONTRACTS },
            new[] { FILES_CONTRACTS },
            new[] { MESSAGES_CONTRACTS },
            new[] { QUOTAS_CONTRACTS },
            new[] { RELATIONSHIPS_CONTRACTS },
            new[] { SYNCHRONIZATION_CONTRACTS },
            new[] { TOKENS_CONTRACTS }
        }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static IObjectProvider<IType> ContractAssembly(string moduleName)
    {
        return Types().That()
            .ResideInAssembly($"Backbone.Modules.{moduleName}.Contracts")
            .As($"{moduleName} Contracts");
    }
}
