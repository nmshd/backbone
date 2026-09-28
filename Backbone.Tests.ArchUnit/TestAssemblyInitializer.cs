using System.Runtime.CompilerServices;

namespace Backbone.Backbone.Tests.ArchUnit;

internal static class TestAssemblyInitializer
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        // Project references only copy assemblies to the output directory; they do not guarantee that the assemblies are loaded.
        // Build the architecture model before any test runs so AppDomain-based discovery sees every Backbone assembly regardless of test execution order.
        _ = Backbone.ARCHITECTURE;
    }
}
