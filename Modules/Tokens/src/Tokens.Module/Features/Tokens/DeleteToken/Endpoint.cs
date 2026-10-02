using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.DeleteToken;

internal static class Endpoint
{
    public static RouteGroupBuilder MapDeleteTokenEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("{id}", Handle)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new Command { Id = id }, cancellationToken);
        return Results.NoContent();
    }
}
