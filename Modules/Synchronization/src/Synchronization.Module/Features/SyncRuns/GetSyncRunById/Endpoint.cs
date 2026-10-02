using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.GetSyncRunById;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetSyncRunByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{id}", Handle)
            .Produces<HttpResponseEnvelopeResult<SyncRunDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query { SyncRunId = id }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
