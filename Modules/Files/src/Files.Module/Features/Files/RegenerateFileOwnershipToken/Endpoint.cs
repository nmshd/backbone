using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Files.Module.Features.Files.RegenerateFileOwnershipToken;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRegenerateFileOwnershipTokenEndpoint(this RouteGroupBuilder group)
    {
        group.MapPatch("{fileId}/RegenerateOwnershipToken", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status403Forbidden)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string fileId, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Command { FileId = fileId }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
