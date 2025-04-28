using Backbone.BuildingBlocks.API.Mvc;
using Backbone.ConsumerApi.Configuration;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Tooling.Extensions;
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

    public OnboardingController(IMediator mediator, IOptions<ConsumerApiConfiguration> configuration) : base(mediator)
    {
        _configuration = configuration.Value;
    }

    [HttpGet("/reference/{referenceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [AllowAnonymous]
    public IActionResult GetReference([FromRoute] string? referenceId, [FromQuery] string? appName)
    {
        // here an optional check to the validity of the reference could be included
        if (!CheckTokenValidity(referenceId))
        {
            const string htmlContent = "<h1>Invalid Request</h1><p>Your token is expired please request a new one from your organization.</p>";
            return base.Content(htmlContent, "text/html");
        }

        var userAgentOfRequest = Request.Headers["User-Agent"].ToString();

        var pickedOnboardingConfiguration = PickConfiguration(appName);

        if (pickedOnboardingConfiguration != null)
        {
            if (IndicatesIPhone(userAgentOfRequest))

                return Redirect(AppendDeviceHint(pickedOnboardingConfiguration.IosAppUrl, I_PHONE_DEVICE_HINT));

            if (IndicatesMacOs(userAgentOfRequest))
                return Redirect(AppendDeviceHint(pickedOnboardingConfiguration.IosAppUrl, MAC_OS_DEVICE_HINT));

            if (IndicatesAndroid(userAgentOfRequest))
                return base.Content(OnboardingPage(Request.Path.Value!, pickedOnboardingConfiguration.AndroidAppUrl, pickedOnboardingConfiguration), "text/html");

            var htmlContent =
                $"<h1>Onboarding</h1><p>Welcome to the onboarding page!</p><a href={pickedOnboardingConfiguration.AndroidAppUrl}>{pickedOnboardingConfiguration.AppNameIdentifier} Playstore</a><br><a href={pickedOnboardingConfiguration.IosAppUrl}>{pickedOnboardingConfiguration.AppNameIdentifier} Appstore</a>";

            return base.Content(htmlContent, "text/html");
        }

        // if we have received null we could not automatically determine the onboarding configuration
        // hence we return a html page with the links to the different onboarding configurations
        var appChooserHtml =
            " <head><title>Onboarding Page</title><style>.color-box {{width: 100px;height: 100px;margin: 10px;display: inline-block;vertical-align: top;text-align: center;line-height: 100px;color: white;font-weight: bold;border-radius: 8px;}}</style></head><p>Please pick the application specified by your provider and install it.</p><br>";

        var baseUrl = HttpContext.Request.PathBase.ToString();

        foreach (var onboardingConfiguration in _configuration.Onboarding)
        {
            appChooserHtml +=
                $"<a href={baseUrl}/Onboarding?appName={onboardingConfiguration.AppNameIdentifier}>Install app {onboardingConfiguration.AppNameIdentifier}</a><br>";
            appChooserHtml +=
                $"<div class='color-box' style='background-color: {onboardingConfiguration.PrimaryColor};'>Primary</div><div class='color-box' style='background-color: {onboardingConfiguration.SecondaryColor};'>Secondary</div><div style='margin-top: 20px;'><img src='{onboardingConfiguration.AppIconUrl}' alt='App Icon' style='width:100px; height:100px;'/></div><br>";
        }

        return base.Content(appChooserHtml, "text/html");
    }

    private bool CheckTokenValidity(string? referenceId)
    {
        if (referenceId.IsNullOrEmpty())
            return false;

        return TokenId.IsValid(referenceId!);
    }

    private ConsumerApiConfiguration.OnboardingConfiguration? PickConfiguration(string? appname)
    {
        var onboardingConfigurations = _configuration.Onboarding;

        // first case - we have no onboarding configuration
        // error should be thrown
        if (onboardingConfigurations.Length == 0)
            throw new ApplicationException("No onboarding configuration found");

        // second case we have only one onboarding configuration
        // use it directly and return the redirect
        if (onboardingConfigurations.Length == 1)
            return onboardingConfigurations[0];

        // third case we have multiple onboarding configurations
        // we need to check is the app name has been specified
        if (appname == null)
            return null;
        else
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

    public string OnboardingPage(string inAppLink, string urlToCorrectAppStore, ConsumerApiConfiguration.OnboardingConfiguration appConfiguration)
    {
        const string header =
            "<head><title>Onboarding Page</title><style>.color-box {width: 100px;height: 100px;margin: 10px;display: inline-block;vertical-align: top;text-align: center;line-height: 100px;color: white;font-weight: bold;border-radius: 8px;}</style></head>";

        const string onboardingText = "<p>You have opend an object with respekt to tokenXX ...  to complete this process do either of the following:</p>";

        const string appAlreadyInstalled = "<p>App already installed?</p> Then please click the link below to open the app directly:<br>";

        const string appStoreLink = "<p>App not yet installed?</p> Then please click the link below to install the app:<br>";

        var designComponentsDisplay =
            $"<div class='color-box' style='background-color: {appConfiguration.PrimaryColor};'>Primary</div><div class='color-box' style='background-color: {appConfiguration.SecondaryColor};'>Secondary</div><div style='margin-top: 20px;'><img src='{appConfiguration.AppIconUrl}' alt='App Icon' style='width:100px; height:100px;'/></div><br>";

        const string jsCodeToReadOutHash =
            "<script>\ndocument.addEventListener('DOMContentLoaded', function () {\n    // Read the fragment identifier (the part after '#')\n    var fragment = window.location.hash;\n\n    // Remove the leading '#' if needed\n    if (fragment.startsWith('#')) {\n        fragment = fragment.substring(1);\n    }\n\n    console.log('Fragment value:', fragment);\n\n    // Now you can use `fragment` however you like!\n});\n</script>";

        return header + jsCodeToReadOutHash + "<br>" + onboardingText + "<br>" + appAlreadyInstalled + "<br>" +
               $"<a href={inAppLink}>Open {appConfiguration.AppNameIdentifier} App</a><br>" +
               appStoreLink + "<br>" +
               $"<a href={urlToCorrectAppStore} target=\"_blank\">Install {appConfiguration.AppNameIdentifier} App</a><br>" +
               designComponentsDisplay;
    }
}
