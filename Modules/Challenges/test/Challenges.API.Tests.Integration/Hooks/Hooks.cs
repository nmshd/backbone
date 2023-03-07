using BoDi;
using Microsoft.Extensions.Configuration;
using SpecFlowCucumberResultsExporter.Extensions;

namespace Challenges.API.Tests.Integration.Hooks;
[Binding]
public sealed class Hooks
{
    // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks

    private const string APP_SETTINGS_FILE = "appsettings.json";

    [BeforeTestRun(Order = 0)]
    public static void BeforeTestRun(IObjectContainer objectContainer)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), APP_SETTINGS_FILE), optional: true, reloadOnChange: true)
            .Build();

        objectContainer.RegisterInstanceAs(config);

        // Export test results to cucumber format
        Exporter.ExportToCucumber();
    }
}