using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.RejectRelationshipReactivation;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRejectRelationshipReactivationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/Reactivate/Reject", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Command { RelationshipId = id }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
