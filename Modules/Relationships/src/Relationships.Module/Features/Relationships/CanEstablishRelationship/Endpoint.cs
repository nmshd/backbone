using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Devices.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CanEstablishRelationship;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCanEstablishRelationshipEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("CanCreate", Handle)
            .Produces<HttpResponseEnvelopeResult<CanEstablishRelationshipResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromQuery(Name = "peer")] string peerAddress, IIdentityStatusProvider identities, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = !await identities.IsActive(peerAddress, cancellationToken)
            ? new CanEstablishRelationshipResponse { CanCreate = false, Code = ApplicationErrors.Relationship.PeerIsToBeDeleted().Code }
            : await mediator.Send(new CanEstablishRelationshipQuery { PeerAddress = peerAddress }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
