using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationship;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRejectRelationshipEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/Reject", Handle)
            .Produces<HttpResponseEnvelopeResult<RejectRelationshipResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromBody] RejectRelationshipRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RejectRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
