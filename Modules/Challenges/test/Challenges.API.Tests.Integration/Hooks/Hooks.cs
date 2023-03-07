using BoDi;
using Challenges.API.Tests.Integration.API;
using Microsoft.Extensions.Configuration;
using RestSharp;
using SpecFlowCucumberResultsExporter.Extensions;
using static Challenges.API.Tests.Integration.Configuration.Settings;

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

        var settings = config.GetSection("Http").Get<HttpConfiguration>() ?? new HttpConfiguration();

        var restClient = new RestClient(settings.BaseUrl);
        var challengesApi = new ChallengesApi(restClient);

        objectContainer.RegisterInstanceAs(config);
        objectContainer.RegisterInstanceAs(challengesApi);

        // Export test results to cucumber format
        Exporter.ExportToCucumber();
    }
}