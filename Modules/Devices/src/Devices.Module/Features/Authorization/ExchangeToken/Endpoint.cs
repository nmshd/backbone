using System.Security.Claims;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Backbone.Modules.Devices.Module.Features.Authorization.ExchangeToken;

internal static class Endpoint
{
    public static IEndpointRouteBuilder MapExchangeTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/connect/token", Handle)
            .AllowAnonymous()
            .ExcludeFromDescription()
            .WithMinimalApiErrorHandling();
        return endpoints;
    }

    private static async Task<IResult> Handle(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        var request = context.GetOpenIddictServerRequest() ??
                      throw new OperationFailedException(ApplicationErrors.Authentication.InvalidOAuthRequest("no request was found"));

        if (!request.IsPasswordGrantType())
            throw new OperationFailedException(ApplicationErrors.Authentication.InvalidOAuthRequest("the specified grant type is not implemented"));

        if (request.Username.IsNullOrEmpty())
            throw new OperationFailedException(ApplicationErrors.Authentication.InvalidOAuthRequest("missing username"));

        var user = await userManager.FindByNameAsync(request.Username!);
        if (user == null || user.Device.Identity.IsGracePeriodOver)
            return InvalidUserCredentials();

        if (request.Password.IsNullOrEmpty())
            throw new OperationFailedException(ApplicationErrors.Authentication.InvalidOAuthRequest("missing password"));

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return UserLockedOut();
        if (!result.Succeeded)
            return InvalidUserCredentials();

        var identity = new ClaimsIdentity(
            claims:
            [
                new(Claims.Subject, user.Id),
                new(Claims.Name, user.UserName!.Trim()),
                new("nmshd_address", user.Device.IdentityAddress),
                new("device_id", user.Device.Id)
            ],
            authenticationType: TokenValidationParameters.DefaultAuthenticationType);

        identity.SetScopes(new[]
        {
            Scopes.OpenId,
            Scopes.Profile
        }.Intersect(request.GetScopes()));

        identity.SetDestinations(GetDestinations);

        return Results.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IResult InvalidUserCredentials()
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                "The username/password couple is invalid."
        });

        return Results.Forbid(properties: properties, authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
    }

    private static IResult UserLockedOut()
    {
        var properties = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.AccessDenied,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                "The user is temporarily locked out, please try again in a few minutes."
        });

        return Results.Forbid(properties: properties, authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
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
