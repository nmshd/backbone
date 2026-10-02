using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Messages.Module.Features.Messages.GetMessage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Messages.Module.Features.Messages.SendMessage;

internal static class Endpoint
{
    public static RouteGroupBuilder MapSendMessageEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);

        return group;
    }

    private static async Task<IResult> Handle([FromBody] Command request, HttpContext context, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return EnvelopeHttpResults.CreatedAtRoute(GetMessage.Endpoint.ROUTE_NAME, new { v = context.Request.RouteValues["v"], id = response.Id }, response);
    }
}
