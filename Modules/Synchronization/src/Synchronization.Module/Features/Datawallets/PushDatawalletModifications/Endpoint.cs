using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Synchronization.Module.Features.Datawallets.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.PushDatawalletModifications;

internal static class Endpoint
{
    public static RouteGroupBuilder MapPushDatawalletModificationsEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("Modifications", Handle)
            .Accepts<PushDatawalletModificationsRequestBody>(isOptional: true, "application/json", "application/*+json")
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] PushDatawalletModificationsRequestBody? request,
        [FromHeader(Name = "X-Supported-Datawallet-Version")] ushort? supportedDatawalletVersion, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request == null || request.Modifications == null || request.Modifications.Any(m => m != null && (m.Collection == null || m.ObjectIdentifier == null)))
            throw new BadHttpRequestException("Required request fields must not be null.");

        var response = await mediator.Send(new Command
        {
            Modifications = request.Modifications, LocalIndex = request.LocalIndex, SupportedDatawalletVersion = supportedDatawalletVersion ?? 0
        }, cancellationToken);
        return EnvelopeHttpResults.Created("", response);
    }
}

public class PushDatawalletModificationsRequestBody
{
    public required long LocalIndex { get; set; }
    public required PushDatawalletModificationItem[] Modifications { get; set; }
}
