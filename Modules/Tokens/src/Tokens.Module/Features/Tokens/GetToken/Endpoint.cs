using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.GetToken;

internal static class Endpoint
{
    public const string ROUTE_NAME = "ConsumerApi.Tokens.GetById";

    public static RouteGroupBuilder MapGetTokenEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{id}", Handle)
            .AllowAnonymous()
            .WithMetadata(new RouteNameMetadata(ROUTE_NAME))
            .Produces<HttpResponseEnvelopeResult<TokenDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromQuery] Base64QueryValue? password, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTokenQuery { Id = id, Password = password?.Value }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
