using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.UpdateTokenContent;

internal static class Endpoint
{
    public static RouteGroupBuilder MapUpdateTokenContentEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("{id}/UpdateContent", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromBody] RequestBody body, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Command { TokenId = id, NewContent = body.NewContent, Password = body.Password }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}

public class RequestBody
{
    public required byte[] NewContent { get; init; }
    public byte[]? Password { get; init; }
}
