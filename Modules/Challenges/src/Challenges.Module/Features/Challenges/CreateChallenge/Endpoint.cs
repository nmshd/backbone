using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.CreateChallenge;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCreateChallengeEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .AllowAnonymous()
            .Produces<HttpResponseEnvelopeResult<ChallengeDTO>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);

        return group;
    }

    private static async Task<IResult> Handle(HttpContext context, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateChallengeCommand(), cancellationToken);
        return EnvelopeHttpResults.CreatedAtRoute(GetChallengeById.Endpoint.ROUTE_NAME, new { v = context.Request.RouteValues["v"], id = response.Id }, response);
    }
}
