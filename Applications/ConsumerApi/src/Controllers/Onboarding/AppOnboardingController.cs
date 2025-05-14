using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCSharp.HttpUserAgentParser;
using MyCSharp.HttpUserAgentParser.Providers;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route("")]
public class AppOnboardingController : Controller
{
    private readonly ConsumerApiConfiguration.AppOnboardingConfiguration? _configuration;
    private readonly IHttpUserAgentParserProvider _parser;

    public AppOnboardingController(IOptions<ConsumerApiConfiguration> configuration, IHttpUserAgentParserProvider parser)
    {
        _configuration = configuration.Value.AppOnboarding;
        _parser = parser;
    }

    [HttpGet("/r/{referenceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public IActionResult GetReference([FromRoute(Name = "referenceId")] string? _, [FromQuery] string? app)
    {
        if (_configuration == null)
            return NotFound();

        app ??= _configuration.DefaultAppId;

        var selectedAppConfiguration = _configuration.Apps.FirstOrDefault(a => a.Id == app);

        if (selectedAppConfiguration == null)
            return View("AppSelection", new AppSelectionModel(_configuration.Apps));

        var appStoreLinks = GetStoreLinksForUserAgent(selectedAppConfiguration);

        return View("AppOnboarding", new AppOnboardingModel(selectedAppConfiguration, appStoreLinks));
    }

    private List<AppOnboardingModel.AppStore> GetStoreLinksForUserAgent(ConsumerApiConfiguration.AppOnboardingConfiguration.App appConfiguration)
    {
        var appStores = new List<AppOnboardingModel.AppStore>();

        var storeTypeForUserAgent = GetStoreTypeForUserAgent();

        switch (storeTypeForUserAgent)
        {
            case StoreType.GooglePlayStore:
                appStores.Add(AppOnboardingModel.AppStore.GooglePlayStore(appConfiguration.GooglePlayStore));
                break;
            case StoreType.AppleAppStore:
                appStores.Add(AppOnboardingModel.AppStore.AppleAppStore(appConfiguration.AppleAppStore));
                break;
            case StoreType.Unknown:
                appStores.Add(AppOnboardingModel.AppStore.GooglePlayStore(appConfiguration.GooglePlayStore));
                appStores.Add(AppOnboardingModel.AppStore.AppleAppStore(appConfiguration.AppleAppStore));
                break;
            default:
                throw new Exception($"The store type {storeTypeForUserAgent} is not supported.");
        }

        return appStores;
    }

    private StoreType GetStoreTypeForUserAgent()
    {
        var userAgent = _parser.Parse(Request.Headers.UserAgent.ToString());
        var platform = userAgent.Platform?.PlatformType;

        return platform switch
        {
            HttpUserAgentPlatformType.Android => StoreType.GooglePlayStore,
            HttpUserAgentPlatformType.IOS => StoreType.AppleAppStore,
            HttpUserAgentPlatformType.MacOS => StoreType.AppleAppStore,
            _ => StoreType.Unknown
        };
    }
}

public class AppSelectionModel
{
    public AppSelectionModel(ConsumerApiConfiguration.AppOnboardingConfiguration.App[] apps)
    {
        Apps = apps.Select(c => new App(c)).ToList();
    }

    public List<App> Apps { get; }

    public class App
    {
        public App(ConsumerApiConfiguration.AppOnboardingConfiguration.App app)
        {
            Id = app.Id;
            DisplayName = app.DisplayName;
        }

        public string Id { get; }
        public string DisplayName { get; }
    }
}

public class AppOnboardingModel
{
    public AppOnboardingModel(ConsumerApiConfiguration.AppOnboardingConfiguration.App config, List<AppStore> links)
    {
        AppId = config.Id;
        AppDisplayName = config.DisplayName;
        Links = links;
    }

    public string AppId { get; }
    public string AppDisplayName { get; }
    public List<AppStore> Links { get; }

    public record AppStore
    {
        private AppStore(string storeName, string? link, string noLinkText)
        {
            StoreName = storeName;
            Link = link;
            NoLinkText = noLinkText;
        }

        public static AppStore GooglePlayStore(ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreConfig config)
        {
            return new AppStore("Google Play Store", config.AppLink, config.NoLinkText);
        }

        public static AppStore AppleAppStore(ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreConfig config)
        {
            return new AppStore("Apple App Store", config.AppLink, config.NoLinkText);
        }

        public string StoreName { get; }
        public string? Link { get; }
        public string NoLinkText { get; }
    }
}

public enum StoreType
{
    GooglePlayStore,
    AppleAppStore,
    Unknown
}
