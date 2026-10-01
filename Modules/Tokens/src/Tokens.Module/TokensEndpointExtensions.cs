using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Tokens.Module.Features.Tokens.CreateToken;
using Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;
using Backbone.Modules.Tokens.Module.Features.Tokens.GetToken;
using Backbone.Modules.Tokens.Module.Features.Tokens.ListTokens;
using Backbone.Modules.Tokens.Module.Features.Tokens.UpdateTokenContent;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Tokens.Module;

public static class TokensEndpointExtensions
{
    public static IEndpointRouteBuilder MapTokensEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Tokens", "Tokens", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        group.MapCreateTokenEndpoint();
        group.MapGetTokenEndpoint();
        group.MapListTokensEndpoint();
        group.MapDeleteTokenEndpoint();
        group.MapUpdateTokenContentEndpoint();
        return endpoints;
    }
}
