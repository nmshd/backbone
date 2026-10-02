using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.GetDatawallet;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetDatawalletEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .Produces<HttpResponseEnvelopeResult<DatawalletDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query(), cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
