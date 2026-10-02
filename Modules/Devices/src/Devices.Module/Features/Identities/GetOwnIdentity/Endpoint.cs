using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.GetOwnIdentity;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetOwnIdentityEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("Self", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK);
        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query(), cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
