using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

internal static class Endpoint
{
    public static RouteGroupBuilder MapFinalizeExternalEventSyncEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/FinalizeExternalEventSync", Handle)
            .Accepts<FinalizeExternalEventSyncRequest>(isOptional: true, "application/json", "application/*+json")
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromBody] FinalizeExternalEventSyncRequest? request, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request == null || request.ExternalEventResults == null || request.DatawalletModifications == null ||
            request.ExternalEventResults.Any(r => r != null && r.ExternalEventId == null) ||
            request.DatawalletModifications.Any(m => m != null && (m.Collection == null || m.ObjectIdentifier == null)))
            throw new BadHttpRequestException("Required request fields must not be null.");

        var response = await mediator.Send(new Command
        {
            SyncRunId = id, ExternalEventResults = request.ExternalEventResults, DatawalletModifications = request.DatawalletModifications
        }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}

public class FinalizeExternalEventSyncRequest
{
    public List<Command.ExternalEventResult> ExternalEventResults { get; set; } = [];
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}
