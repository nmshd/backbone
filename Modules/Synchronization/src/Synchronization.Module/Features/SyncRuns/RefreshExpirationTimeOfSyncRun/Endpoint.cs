using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.RefreshExpirationTimeOfSyncRun;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRefreshExpirationTimeOfSyncRunEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/RefreshExpirationTime", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Command { SyncRunId = id }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
