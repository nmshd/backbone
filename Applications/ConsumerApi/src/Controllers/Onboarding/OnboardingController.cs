using Backbone.BuildingBlocks.API.Mvc;
using Backbone.ConsumerApi.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backbone.ConsumerApi.Controllers.Onboarding;

[ApiController]
[Route("[controller]")]
public class OnboardingController : ApiControllerBase
{
    private readonly ConsumerApiConfiguration _configuration;

    public OnboardingController(IMediator mediator, IOptions<ConsumerApiConfiguration> configuration) : base(mediator)
    {
        _configuration = configuration.Value;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        // Pretend to be async to fit framework
        await Task.Yield();

        var userAgentOfRequest = Request.Headers["User-Agent"].ToString();

        if (IndicatesMacOs(userAgentOfRequest)) return Redirect(_configuration.Onboarding.IosAppUrl);

        if (IndicatesAndroid(userAgentOfRequest)) return Redirect(_configuration.Onboarding.AndroidAppUrl);

        const string htmlContent =
            "<h1>Onboarding</h1><p>Welcome to the onboarding page!</p><a href=https://play.google.com/store/apps/details?id=eu.enmeshed.app&hl=de>Enmeshed Playstore</a><a href=https://apps.apple.com/us/app/enmeshed/id1576693742?platform=iphone>Enmeshed Appstore</a>";

        return base.Content(htmlContent, "text/html");
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
