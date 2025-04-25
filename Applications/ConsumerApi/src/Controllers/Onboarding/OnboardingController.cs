using Backbone.BuildingBlocks.API.Mvc;
using Backbone.ConsumerApi.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[Route("Tokens")]
public class OnboardingController : ApiControllerBase
{
    private readonly ConsumerApiConfiguration _configuration;

    public OnboardingController(IMediator mediator, IOptions<ConsumerApiConfiguration> configuration) : base(mediator)
    {
        _configuration = configuration.Value;
    }

    [HttpGet("{tokenId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [AllowAnonymous]
    public IActionResult Get([FromRoute] string? tokenId)
    {
        return Get(tokenId, null);
    }

    [HttpGet("{tokenId}/{appname}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [AllowAnonymous]
    public IActionResult Get([FromRoute] string? tokenId, [FromRoute] string? appname)
    {
        var userAgentOfRequest = Request.Headers["User-Agent"].ToString();

        var pickedOnboardingConfiguration = PickConfiguration(appname);

        if (pickedOnboardingConfiguration != null)
        {
            if (IndicatesMacOs(userAgentOfRequest))
                return Redirect(pickedOnboardingConfiguration.IosAppUrl);

            if (IndicatesAndroid(userAgentOfRequest))
                return Redirect(pickedOnboardingConfiguration.AndroidAppUrl);

            var htmlContent =
                $"<h1>Onboarding</h1><p>Welcome to the onboarding page!</p><a href={pickedOnboardingConfiguration.AndroidAppUrl}>{pickedOnboardingConfiguration.AppNameIdentifier} Playstore</a><br><a href={pickedOnboardingConfiguration.IosAppUrl}>{pickedOnboardingConfiguration.AppNameIdentifier} Appstore</a>";

            return base.Content(htmlContent, "text/html");
        }

        // if we have received null we could not automatically determine the onboarding configuration
        // hence we return a html page with the links to the different onboarding configurations

        var appChooserHtml = "<h1>Onboarding</h1><p>Welcome to the onboarding page!</p><br>";

        var baseUrl = HttpContext.Request.PathBase.ToString();

        foreach (var onboardingConfiguration in _configuration.Onboarding)
            appChooserHtml +=
                $"<a href={baseUrl}/Tokens/{tokenId}/{onboardingConfiguration.AppNameIdentifier}>Install app {onboardingConfiguration.AppNameIdentifier}</a><br>";

        return base.Content(appChooserHtml, "text/html");
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
        return userAgentContent.Contains("Android");
    }

    private bool IndicatesMacOs(string userAgentContent)
    {
        if (userAgentContent.Contains("Mac OS"))
            return true;
        if (userAgentContent.Contains("iPhone"))
            return true;
        if (userAgentContent.Contains("Macintosh"))
            return true;

        return false;
    }
}
