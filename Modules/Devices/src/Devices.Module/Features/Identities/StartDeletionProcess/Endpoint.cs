using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.StartDeletionProcess;

internal static class Endpoint
{
    public static RouteGroupBuilder MapStartDeletionProcessEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] Command? request, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request ?? new Command(), cancellationToken);
        return EnvelopeHttpResults.Created("", response);
    }
}
