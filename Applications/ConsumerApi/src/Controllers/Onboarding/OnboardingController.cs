using Backbone.ConsumerApi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route("")]
public class OnboardingController : Controller
{
    // TODO: read from configuration
    private readonly AppSelectionModel _appSelectionModel = new()
    {
        Apps =
        [
            new App
            {
                Identifier = "enmeshed",
                DisplayName = "enmeshed",
            },
            new App
            {
                Identifier = "bird",
                DisplayName = "BIRD",
            }
        ]
    };

    private const string IPHONE_IDENTIFIER = "iphone";
    private const string ANDROID_IDENTIFIER = "android";
    private const string MACINTOSH_IDENTIFIER = "macintosh";
    private const string IOS_IDENTIFIER = "mac os";

    private const string I_PHONE_DEVICE_HINT = "iphone";
    private const string MAC_OS_DEVICE_HINT = "ipad";

    private readonly ConsumerApiConfiguration _configuration;

    public OnboardingController(IOptions<ConsumerApiConfiguration> configuration)
    {
        _configuration = configuration.Value;
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
            var userAgentOfRequest = Request.Headers["User-Agent"].ToString();

            // Note: this order is important as iPhoneRequest will match with MacOsRequest also.
            if (IndicatesIPhone(userAgentOfRequest))
                appStoreLinks.Add(new AppStoreLink("Apple App Store", AppendDeviceHint(pickedOnboardingConfiguration.IosAppUrl, I_PHONE_DEVICE_HINT)));
            else if (IndicatesMacOs(userAgentOfRequest))
                appStoreLinks.Add(new AppStoreLink("Apple App Store", AppendDeviceHint(pickedOnboardingConfiguration.IosAppUrl, MAC_OS_DEVICE_HINT)));

            if (IndicatesAndroid(userAgentOfRequest))
                appStoreLinks.Add(new AppStoreLink("Google Play Store", pickedOnboardingConfiguration.AndroidAppUrl));

            if (appStoreLinks.Count == 0)
            {
                appStoreLinks.Add(new AppStoreLink("Google Play Store", pickedOnboardingConfiguration.AndroidAppUrl));
                appStoreLinks.Add(new AppStoreLink("Apple App Store", pickedOnboardingConfiguration.IosAppUrl));
            }

            if (string.IsNullOrEmpty(Request.Path.Value)) return BadRequest("Invalid request path");


            var onboardingModel = new OnboardingModel
            {
                // TODO: read from configuration
                AppDisplayName = _appSelectionModel.Apps.First(a => a.Identifier == appName).DisplayName,
                Links = appStoreLinks
            };

            return View("Onboarding", onboardingModel);
        }

        return View("AppSelection", _appSelectionModel);
    }

    private ConsumerApiConfiguration.OnboardingConfiguration? PickAppSpecificConfiguration(string? appname)
    {
        var onboardingConfigurations = _configuration.Onboarding;

        // the length can't be null, as it is required by the configuration

        if (onboardingConfigurations.Length == 1)
            return onboardingConfigurations[0];

        foreach (var appConfig in onboardingConfigurations)
            if (appConfig.AppNameIdentifier.Equals(appname))
                return appConfig;

        return null;
    }

    private bool IndicatesAndroid(string userAgentContent)
    {
        return userAgentContent.ToLower().Contains(ANDROID_IDENTIFIER);
    }

    private bool IndicatesMacOs(string userAgentContent)
    {
        if (userAgentContent.ToLower().Contains(IOS_IDENTIFIER))
            return true;
        if (userAgentContent.ToLower().Contains(MACINTOSH_IDENTIFIER))
            return true;

        return false;
    }

    private bool IndicatesIPhone(string userAgentContent)
    {
        return userAgentContent.ToLower().Contains(IPHONE_IDENTIFIER);
    }

    private string AppendDeviceHint(string url, string deviceHint)
    {
        return url.Contains('?') ? $"{url}&platform={deviceHint}" : $"{url}?platform={deviceHint}";
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
