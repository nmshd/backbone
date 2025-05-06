using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MyCSharp.HttpUserAgentParser;
using MyCSharp.HttpUserAgentParser.Providers;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route("")]
public class OnboardingController : Controller
{
    private const string IPHONE_DEVICE_HINT = "iphone";
    private const string MAC_OS_DEVICE_HINT = "ipad";

    private readonly ConsumerApiConfiguration _configuration;
    private readonly IHttpUserAgentParserProvider _parser;

    public OnboardingController(IOptions<ConsumerApiConfiguration> configuration, IHttpUserAgentParserProvider parser)
    {
        _configuration = configuration.Value;
        _parser = parser;
    }

    [HttpGet("/reference/{referenceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public IActionResult GetReference([FromRoute] string? referenceId, [FromQuery] string? appName)
    {
        var pickedOnboardingConfiguration = PickAppSpecificConfiguration(appName);

        if (pickedOnboardingConfiguration != null)
        {
            var appStoreLinks = new List<AppStoreLink>();
            var userAgentInformation = _parser.Parse(Request.Headers.UserAgent.ToString());

            var androidAppUrl = pickedOnboardingConfiguration.Android.Url;
            var iosAppUrl = pickedOnboardingConfiguration.Ios.Url;

            if (userAgentInformation.Platform != null)
                switch (userAgentInformation.Platform!.Value.PlatformType)
                {
                    case HttpUserAgentPlatformType.Android:
                        appStoreLinks.Add(new AppStoreLink("Google Play Store", androidAppUrl));
                        break;
                    case HttpUserAgentPlatformType.IOS:
                        appStoreLinks.Add(new AppStoreLink("Apple App Store", AppendDeviceHint(iosAppUrl, IPHONE_DEVICE_HINT)));
                        break;
                    case HttpUserAgentPlatformType.MacOS:
                        appStoreLinks.Add(new AppStoreLink("Apple App Store", AppendDeviceHint(iosAppUrl, MAC_OS_DEVICE_HINT)));
                        break;
                }

            if (appStoreLinks.Count == 0)
            {
                appStoreLinks.Add(new AppStoreLink("Google Play Store", androidAppUrl));
                appStoreLinks.Add(new AppStoreLink("Apple App Store", iosAppUrl));
            }

            if (string.IsNullOrEmpty(Request.Path.Value)) return BadRequest("Invalid request path");


            var onboardingModel = CreateOnbordingModel(pickedOnboardingConfiguration, appStoreLinks);

            return View("Onboarding", onboardingModel);
        }

        return View("AppSelection", CreateAppSelectionModel());
    }

    private ConsumerApiConfiguration.App? PickAppSpecificConfiguration(string? appName)
    {
        var appConfigurations = _configuration.Onboarding.Apps;

        if (appConfigurations.Length == 1)
            return appConfigurations[0];

        return appConfigurations.FirstOrDefault(c => c.Identifier.Equals(appName));
    }

    private string AppendDeviceHint(string url, string deviceHint)
    {
        return QueryHelpers.AddQueryString(url, "platform", deviceHint);
    }

    private AppSelectionModel CreateAppSelectionModel()
    {
        var apps = new List<App>();
        foreach (var appConfig in _configuration.Onboarding.Apps)
        {
            var app = new App
            {
                Identifier = appConfig.Identifier,
                DisplayName = appConfig.DisplayName
            };
            apps.Add(app);
        }

        return new AppSelectionModel
        {
            Apps = apps
        };
    }

    private OnboardingModel CreateOnbordingModel(ConsumerApiConfiguration.App appConfig, List<AppStoreLink> appStoreLinks)
    {
        return new OnboardingModel
        {
            AppDisplayName = appConfig.DisplayName,
            Links = appStoreLinks
        };
    }
}

public class AppSelectionModel
{
    public required List<App> Apps { get; set; }
}

public class App
{
    public required string Identifier { get; set; }
    public required string DisplayName { get; set; }
}

public class OnboardingModel
{
    public required string AppDisplayName { get; set; }
    public required List<AppStoreLink> Links { get; set; }
}

public record AppStoreLink
{
    public AppStoreLink(string storeName, string link)
    {
        StoreName = storeName;
        Link = link;
    }

    public string StoreName { get; set; }
    public string Link { get; set; }
}
