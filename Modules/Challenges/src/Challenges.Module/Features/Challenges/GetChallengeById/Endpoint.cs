using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Challenges.Module.Features.Challenges.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Challenges.Module.Features.Challenges.GetChallengeById;

internal static class Endpoint
{
    public const string ROUTE_NAME = "ConsumerApi.Challenges.GetById";

    public static RouteGroupBuilder MapGetChallengeByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{id}", Handle)
            // Name the Location target without adding an OpenAPI operationId.
            .WithMetadata(new RouteNameMetadata(ROUTE_NAME))
            .Produces<HttpResponseEnvelopeResult<ChallengeDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> Handle(string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetChallengeByIdQuery { Id = id }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
