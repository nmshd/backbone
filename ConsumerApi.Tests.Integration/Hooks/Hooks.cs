using Backbone.SpecFlowCucumberResultsExporter.Extensions;

namespace Backbone.ConsumerApi.Tests.Integration.Hooks;
[Binding]
public sealed class Hooks
{
    [BeforeTestRun(Order = 0)]
    public static void BeforeTestRun()
    {
        // Export test results to cucumber format
        Exporter.ExportToCucumber();
    }
}
