using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Challenges.Module.Features.Challenges.CreateChallenge;
using Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Challenges.Module;

public static class ChallengesEndpointExtensions
{
    public static IEndpointRouteBuilder MapChallengesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapVersionedEndpointGroup("Challenges", "Challenges", 2, OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        group.MapCreateChallengeEndpoint();
        group.MapGetChallengeByIdEndpoint();
        return endpoints;
    }
}
