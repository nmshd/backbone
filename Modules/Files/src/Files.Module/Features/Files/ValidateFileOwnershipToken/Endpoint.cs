using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Files.Module.Features.Files.ValidateFileOwnershipToken;

internal static class Endpoint
{
    public static RouteGroupBuilder MapValidateFileOwnershipTokenEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("{fileId}/ValidateOwnershipToken", Handle)
            .Produces<HttpResponseEnvelopeResult<ValidateFileOwnershipTokenResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status403Forbidden)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string fileId, [FromBody] RequestBody body, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ValidateFileOwnershipTokenQuery { FileId = fileId, OwnershipToken = body.OwnershipToken }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }

    private class RequestBody
    {
        public required string OwnershipToken { get; init; }
    }
}
