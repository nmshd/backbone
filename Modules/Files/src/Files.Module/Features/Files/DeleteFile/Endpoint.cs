using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Files.Module.Features.Files.DeleteFile;

internal static class Endpoint
{
    public static RouteGroupBuilder MapDeleteFileEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("{fileId}", Handle)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string fileId, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new Command { Id = fileId }, cancellationToken);
        return Results.NoContent();
    }
}
