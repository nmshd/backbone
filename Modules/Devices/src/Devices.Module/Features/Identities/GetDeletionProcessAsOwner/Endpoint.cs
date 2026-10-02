using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Devices.Module.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsOwner;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetDeletionProcessAsOwnerEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{id}", Handle)
            .Produces<HttpResponseEnvelopeResult<IdentityDeletionProcessOverviewDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetDeletionProcessAsOwnerQuery { Id = id }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
