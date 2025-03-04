using System.Security.Claims;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling.Extensions;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.ConsumerApi.Controllers;

[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class AuthorizationController : ApiControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthorizationController(
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager) : base(mediator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("~/connect/token")]
    [Produces("application/json")]
    [Consumes("application/x-www-form-urlencoded")]
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new OperationFailedException(
            ApplicationErrors.Authentication.InvalidOAuthRequest("no request was found"));

        if (!request.IsPasswordGrantType())
            throw new OperationFailedException(
                ApplicationErrors.Authentication.InvalidOAuthRequest("the specified grant type is not implemented"));

        if (request.Username.IsNullOrEmpty())
            throw new OperationFailedException(
                ApplicationErrors.Authentication.InvalidOAuthRequest("missing username"));

        var user = await _userManager.FindByNameAsync(request.Username!);
        if (user == null || user.Device.Identity.IsGracePeriodOver)
            return InvalidUserCredentials();

        if (request.Password.IsNullOrEmpty())
            throw new OperationFailedException(
                ApplicationErrors.Authentication.InvalidOAuthRequest("missing password"));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return UserLockedOut();
        if (!result.Succeeded)
            return InvalidUserCredentials();

        var identity = new ClaimsIdentity(
            claims:
            [
                new(Claims.Subject, user.Id),
                new(Claims.Name, user.UserName!.Trim()),
                new("address", user.Device.Identity.Address),
                new("device_id", user.Device.Id)
            ],
            authenticationType: TokenValidationParameters.DefaultAuthenticationType);

        identity.SetScopes(new[]
        {
            Scopes.OpenId,
            Scopes.Profile
        }.Intersect(request.GetScopes()));

        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private ForbidResult InvalidUserCredentials()
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                "The username/password couple is invalid."
        });

        return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private ForbidResult UserLockedOut()
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                "The user is temporarily locked out, please try again in a few minutes."
        });

        return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        if (claim.Type == "AspNet.Identity.SecurityStamp")
        {
            yield break;
        }

        yield return Destinations.AccessToken;
    }
}
