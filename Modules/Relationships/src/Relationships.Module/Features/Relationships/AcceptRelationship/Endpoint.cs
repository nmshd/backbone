using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationship;

internal static class Endpoint
{
    public static RouteGroupBuilder MapAcceptRelationshipEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/Accept", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromBody] RequestBody body, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Command { RelationshipId = id, CreationResponseContent = body.CreationResponseContent }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("AcceptRelationshipRequest")]
public class RequestBody
{
    public byte[]? CreationResponseContent { get; set; } = [];
}
