using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.IsIdentityOfUserDeleted;

internal static class Endpoint
{
    public static RouteGroupBuilder MapIsIdentityOfUserDeletedEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("IsDeleted", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .AllowAnonymous();
        return group;
    }

    private static async Task<IResult> Handle([FromQuery(Name = "username")] string username, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query { Username = username }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
