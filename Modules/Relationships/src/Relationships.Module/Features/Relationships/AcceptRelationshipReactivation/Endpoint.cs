using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.AcceptRelationshipReactivation;

internal static class Endpoint
{
    public static RouteGroupBuilder MapAcceptRelationshipReactivationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/Reactivate/Accept", Handle)
            .Produces<HttpResponseEnvelopeResult<AcceptRelationshipReactivationResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new AcceptRelationshipReactivationCommand { RelationshipId = id }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
