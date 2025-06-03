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

        var stores = ListStoresForUserAgent(selectedAppConfiguration);

        return View("AppOnboarding", new AppOnboardingModel(selectedAppConfiguration, stores));
    }

    private List<AppOnboardingModel.AppStore> ListStoresForUserAgent(ConsumerApiConfiguration.AppOnboardingConfiguration.App appConfiguration)
    {
        var stores = new List<AppOnboardingModel.AppStore>();

        var storeTypeForUserAgent = GetStoreTypeForUserAgent();

        switch (storeTypeForUserAgent)
        {
            case StoreType.GooglePlayStore:
                stores.Add(AppOnboardingModel.AppStore.GooglePlayStore(appConfiguration.GooglePlayStore));
                break;
            case StoreType.AppleAppStore:
                stores.Add(AppOnboardingModel.AppStore.AppleAppStore(appConfiguration.AppleAppStore));
                break;
            case StoreType.Unknown:
                stores.Add(AppOnboardingModel.AppStore.GooglePlayStore(appConfiguration.GooglePlayStore));
                stores.Add(AppOnboardingModel.AppStore.AppleAppStore(appConfiguration.AppleAppStore));
                break;
            default:
                throw new Exception($"The store type {storeTypeForUserAgent} is not supported.");
        }

        return stores;
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
            AppIconUrl = app.IconUrl;
            BackgroundColor = app.BackgroundColor;
            PrimaryColor = app.PrimaryColor;
            SecondaryColor = app.SecondaryColor;
        }

        public string Id { get; }
        public string DisplayName { get; }

        public string AppIconUrl { get; }
        public string BackgroundColor { get; }
        public string PrimaryColor { get; }
        public string SecondaryColor { get; }
    }
}

public class AppOnboardingModel
{
    public AppOnboardingModel(ConsumerApiConfiguration.AppOnboardingConfiguration.App config, List<AppStore> links)
    {
        AppId = config.Id;
        AppDisplayName = config.DisplayName;
        Links = links;
        BackgroundColor = config.BackgroundColor;
        PrimaryColor = config.PrimaryColor;
        SecondaryColor = config.SecondaryColor;
        BannerUrl = config.BannerUrl;
        AppIconUrl = config.IconUrl;
    }

    public string AppId { get; }
    public string AppDisplayName { get; }
    public List<AppStore> Links { get; }
    public string BackgroundColor { get; }
    public string PrimaryColor { get; }
    public string SecondaryColor { get; }
    public string BannerUrl { get; }
    public string AppIconUrl { get; }
    public string AppStoreDescriptor => Links.Count == 1 ? Links[0].StoreName : "App Store";

    public record AppStore
    {
        private AppStore(string storeName, string? link, string noLinkText, string storeButtonUrl)
        {
            StoreName = storeName;
            Link = link;
            NoLinkText = noLinkText;
            StoreButtonUrl = storeButtonUrl;
        }

        public static AppStore GooglePlayStore(ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreConfig config)
        {
            return new AppStore("Google Play Store", config.AppLink, config.NoLinkText, config.StoreButtonUrl);
        }

        public static AppStore AppleAppStore(ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreConfig config)
        {
            return new AppStore("Apple App Store", config.AppLink, config.NoLinkText, config.StoreButtonUrl);
        }

        public string StoreName { get; }
        public string? Link { get; }
        public string NoLinkText { get; }
        public string StoreButtonUrl { get; }
    }
}

public enum StoreType
{
    GooglePlayStore,
    AppleAppStore,
    Unknown
}
