using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.CreateToken;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCreateTokenEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .AllowAnonymous()
            .Produces<HttpResponseEnvelopeResult<CreateTokenResponse>>(StatusCodes.Status201Created);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] CreateTokenCommand request, HttpContext context, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return EnvelopeHttpResults.CreatedAtRoute(GetToken.Endpoint.ROUTE_NAME, new { v = context.Request.RouteValues["v"], id = response.Id }, response);
    }
}
