using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListFeatureFlags;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListFeatureFlagsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{identityAddress}/FeatureFlags", Handle)
            .Produces<HttpResponseEnvelopeResult<ListFeatureFlagsResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string identityAddress, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ListFeatureFlagsQuery { IdentityAddress = identityAddress }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
