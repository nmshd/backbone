using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Messages.Module.Features.Messages.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Messages.Module.Features.Messages.GetMessage;

internal static class Endpoint
{
    public const string ROUTE_NAME = "ConsumerApi.Messages.GetMessage";

    public static RouteGroupBuilder MapGetMessageEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{id}", Handle)
            .WithName(ROUTE_NAME)
            .Produces<HttpResponseEnvelopeResult<MessageDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<IResult> Handle(string id, bool? noBody, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query { Id = id, NoBody = noBody == true }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
