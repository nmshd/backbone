using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RevokeRelationship;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRevokeRelationshipEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/Revoke", Handle)
            .Produces<HttpResponseEnvelopeResult<RevokeRelationshipResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromBody] RevokeRelationshipRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RevokeRelationshipCommand { RelationshipId = id, CreationResponseContent = request.CreationResponseContent }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
