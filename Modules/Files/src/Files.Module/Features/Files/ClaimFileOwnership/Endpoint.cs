using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Files.Module.Features.Files.ClaimFileOwnership;

internal static class Endpoint
{
    public static RouteGroupBuilder MapClaimFileOwnershipEndpoint(this RouteGroupBuilder group)
    {
        group.MapPatch("{fileId}/ClaimOwnership", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status403Forbidden)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string fileId, [FromBody] RequestBody body, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Command { FileId = fileId, OwnershipToken = body.OwnershipToken }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}

public class RequestBody
{
    public required string OwnershipToken { get; init; }
}
