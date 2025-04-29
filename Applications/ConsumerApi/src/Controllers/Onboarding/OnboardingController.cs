using Backbone.BuildingBlocks.API.Mvc;
using Backbone.ConsumerApi.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route("")]
public class OnboardingController : ApiControllerBase
{
    private const string IPHONE_IDENTIFIER = "iphone";
    private const string ANDROID_IDENTIFIER = "android";
    private const string MACINTOSH_IDENTIFIER = "macintosh";
    private const string IOS_IDENTIFIER = "mac os";

    private const string I_PHONE_DEVICE_HINT = "iphone";
    private const string MAC_OS_DEVICE_HINT = "ipad";

    private readonly ConsumerApiConfiguration _configuration;
    private readonly OnboardingHtmlFactory _onboardingHtmlFactory = new();

    public OnboardingController(IMediator mediator, IOptions<ConsumerApiConfiguration> configuration) : base(mediator)
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
            var appStoreList = new List<Tuple<string, string>>();
            var userAgentOfRequest = Request.Headers["User-Agent"].ToString();

            // Note: this order is important as iPhoneRequest will match with MacOsRequest also.
            if (IndicatesIPhone(userAgentOfRequest))
                appStoreList.Add(new Tuple<string, string>("Apple App Store", AppendDeviceHint(pickedOnboardingConfiguration.IosAppUrl, I_PHONE_DEVICE_HINT)));
            else if (IndicatesMacOs(userAgentOfRequest))
                appStoreList.Add(new Tuple<string, string>("Apple App Store", AppendDeviceHint(pickedOnboardingConfiguration.IosAppUrl, MAC_OS_DEVICE_HINT)));

            if (IndicatesAndroid(userAgentOfRequest))
                appStoreList.Add(new Tuple<string, string>("Google Play Store", pickedOnboardingConfiguration.AndroidAppUrl));

            if (appStoreList.Count == 0)
            {
                appStoreList.Add(new Tuple<string, string>("Google Play Store", pickedOnboardingConfiguration.AndroidAppUrl));
                appStoreList.Add(new Tuple<string, string>("Apple App Store", pickedOnboardingConfiguration.IosAppUrl));
            }

            if (string.IsNullOrEmpty(Request.Path.Value)) return BadRequest("Invalid request path");

            return _onboardingHtmlFactory.GenerateOnboardingPage(pickedOnboardingConfiguration, appStoreList);
        }

        return _onboardingHtmlFactory.GenerateAppSelectionPage(_configuration, Request);
    }

    private ConsumerApiConfiguration.OnboardingConfiguration? PickAppSpecificConfiguration(string? appname)
    {
        var onboardingConfigurations = _configuration.Onboarding;

        // the length can't be null, as it is required by the configuration

        if (onboardingConfigurations.Length == 1)
            return onboardingConfigurations[0];

        foreach (var appconfig in onboardingConfigurations)
            if (appconfig.AppNameIdentifier.Equals(appname!))
                return appconfig;

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
        if (url.Contains("?"))
            return url + "&platform=" + deviceHint;

        return url + "?platform=" + deviceHint;
    }
}
