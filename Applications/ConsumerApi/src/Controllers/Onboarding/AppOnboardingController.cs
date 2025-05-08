using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyCSharp.HttpUserAgentParser;
using MyCSharp.HttpUserAgentParser.Providers;
using PlatformType = Backbone.ConsumerApi.Configuration.ConsumerApiConfiguration.AppOnboardingConfiguration.App.PlatformType;

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

        var appStoreLinks = GetAppStoreLinksForCurrentUserAgent(selectedAppConfiguration);

        return View("AppOnboarding", new AppOnboardingModel(selectedAppConfiguration.Id, selectedAppConfiguration.DisplayName, appStoreLinks));
    }

    private List<AppOnboardingModel.AppStoreLink> GetAppStoreLinksForCurrentUserAgent(ConsumerApiConfiguration.AppOnboardingConfiguration.App appConfiguration)
    {
        var appStoreLinks = new List<AppOnboardingModel.AppStoreLink>();

        var platform = GetPlatformFromUserAgent();
        var allLinks = appConfiguration.GetAllConfiguredAppStoreLinks();

        if (allLinks.TryGetValue(platform, out var link))
            appStoreLinks.Add(AppOnboardingModel.AppStoreLink.From(platform, link));
        else
            appStoreLinks.AddRange(allLinks.Select(kv => AppOnboardingModel.AppStoreLink.From(kv.Key, kv.Value)));

        return appStoreLinks;
    }

    private PlatformType GetPlatformFromUserAgent()
    {
        var userAgent = _parser.Parse(Request.Headers.UserAgent.ToString());
        var platform = userAgent.Platform?.PlatformType;

        return platform switch
        {
            HttpUserAgentPlatformType.Android => PlatformType.Android,
            HttpUserAgentPlatformType.IOS => PlatformType.Ios,
            HttpUserAgentPlatformType.MacOS => PlatformType.Macos,
            _ => PlatformType.Unknown
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

        public static AppStoreLink From(PlatformType platformType, string link)
        {
            return platformType switch
            {
                PlatformType.Android => Android(link),
                PlatformType.Ios => Ios(link),
                PlatformType.Macos => MacOs(link),
                _ => throw new ArgumentOutOfRangeException(nameof(platformType), platformType, null)
            };
        }

        private static AppStoreLink Android(string link)
        {
            return new AppStoreLink("Google Play Store", link);
        }

        private static AppStoreLink Ios(string link)
        {
            return new AppStoreLink("Apple App Store - iPhone", link);
        }

        private static AppStoreLink MacOs(string link)
        {
            return new AppStoreLink("Apple App Store - macOS", link);
        }

        public string StoreName { get; }
        public string Link { get; }
    }
}
