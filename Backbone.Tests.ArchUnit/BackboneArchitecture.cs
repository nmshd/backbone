using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace Backbone.Backbone.Tests.ArchUnit;

public static class Backbone
{
    public static readonly Architecture ARCHITECTURE =
        new ArchLoader()
            .LoadAssemblies(GetSolutionAssemblies())
            .Build();

    public static readonly IObjectProvider<IType> TEST_TYPES =
        Types().That().ResideInAssemblyMatching(".*Tests.*");

    private static Assembly[] GetSolutionAssemblies()
    {
        var assemblies = Directory
            .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(x => !x.Contains("xunit")) // Not excluding xunit assemblies causes issues when running the tests with `dotnet test`. But they aren't necessary anyway.
            .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
            .Where(x => x.FullName!.StartsWith("Backbone"));
        return assemblies.ToArray();
    }
}
