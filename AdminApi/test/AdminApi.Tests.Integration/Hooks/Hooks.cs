using Backbone.SpecFlowCucumberResultsExporter.Extensions;

namespace Backbone.AdminApi.Tests.Integration.Hooks;

[Binding]
public sealed class Hooks
{
    // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

    [BeforeTestRun(Order = 0)]
    public static void BeforeTestRun()
    {
        // Export test results to cucumber format
        Exporter.ExportToCucumber();
    }
}
