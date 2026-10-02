using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using FinalizeExternalEventSync = Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeExternalEventSync;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.FinalizeDatawalletVersionUpgrade;

internal static class Endpoint
{
    public static RouteGroupBuilder MapFinalizeDatawalletVersionUpgradeEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("{id}/FinalizeDatawalletVersionUpgrade", Handle)
            .Accepts<FinalizeDatawalletVersionUpgradeRequest>(isOptional: true, "application/json", "application/*+json")
            .Produces<HttpResponseEnvelopeResult<FinalizeExternalEventSync.Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromBody] FinalizeDatawalletVersionUpgradeRequest? request, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request == null || request.DatawalletModifications == null || request.DatawalletModifications.Any(m => m != null && (m.Collection == null || m.ObjectIdentifier == null)))
            throw new BadHttpRequestException("Required request fields must not be null.");

        var response = await mediator.Send(new Command
        {
            SyncRunId = id, NewDatawalletVersion = request.NewDatawalletVersion, DatawalletModifications = request.DatawalletModifications
        }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}

public class FinalizeDatawalletVersionUpgradeRequest
{
    public ushort NewDatawalletVersion { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}
