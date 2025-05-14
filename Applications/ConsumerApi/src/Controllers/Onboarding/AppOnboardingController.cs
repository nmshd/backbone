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

        var appStoreLinks = GetStoreLinksForCurrentUserAgent(selectedAppConfiguration);

        return View("AppOnboarding", new AppOnboardingModel(selectedAppConfiguration.Id, selectedAppConfiguration.DisplayName, appStoreLinks));
    }

    private List<AppOnboardingModel.AppStoreLink> GetStoreLinksForCurrentUserAgent(ConsumerApiConfiguration.AppOnboardingConfiguration.App appConfiguration)
    {
        var storeLinks = new List<AppOnboardingModel.AppStoreLink>();

        var platform = GetStoreTypeForUserAgent();
        var allLinks = appConfiguration.GetAllConfiguredStoreLinks();

        if (allLinks.TryGetValue(platform, out var link))
            storeLinks.Add(AppOnboardingModel.AppStoreLink.From(platform, link));
        else
            storeLinks.AddRange(allLinks.Select(kv => AppOnboardingModel.AppStoreLink.From(kv.Key, kv.Value)));

        return storeLinks;
    }

    private ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType GetStoreTypeForUserAgent()
    {
        var userAgent = _parser.Parse(Request.Headers.UserAgent.ToString());
        var platform = userAgent.Platform?.PlatformType;

        return platform switch
        {
            HttpUserAgentPlatformType.Android => ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType.GooglePlayStore,
            HttpUserAgentPlatformType.IOS => ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType.AppleAppStore,
            HttpUserAgentPlatformType.MacOS => ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType.AppleAppStore,
            _ => ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType.Unknown
        };
    }
}

public class AppSelectionModel
{
    public AppSelectionModel(ConsumerApiConfiguration.AppOnboardingConfiguration.App[] apps)
    {
        Apps = apps.Select(c => new App { Id = c.Id, DisplayName = c.DisplayName }).ToList();
    }

    public List<App> Apps { get; }

    public class App
    {
        public required string Id { get; init; }
        public required string DisplayName { get; init; }
    }
}

public class AppOnboardingModel
{
    public AppOnboardingModel(string appId, string appDisplayName, List<AppStoreLink> links)
    {
        AppId = appId;
        AppDisplayName = appDisplayName;
        Links = links;
    }

    public string AppId { get; }
    public string AppDisplayName { get; }
    public List<AppStoreLink> Links { get; }

    public record AppStoreLink
    {
        private AppStoreLink(string storeName, string link)
        {
            StoreName = storeName;
            Link = link;
        }

        public static AppStoreLink From(ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType storeType, string link)
        {
            return storeType switch
            {
                ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType.GooglePlayStore => GooglePlayStore(link),
                ConsumerApiConfiguration.AppOnboardingConfiguration.App.StoreType.AppleAppStore => AppleAppStore(link),
                _ => throw new ArgumentOutOfRangeException(nameof(storeType), storeType, null)
            };
        }

        private static AppStoreLink GooglePlayStore(string link)
        {
            return new AppStoreLink("Google Play Store", link);
        }

        private static AppStoreLink AppleAppStore(string link)
        {
            return new AppStoreLink("Apple App Store", link);
        }

        public string StoreName { get; }
        public string Link { get; }
    }
}
