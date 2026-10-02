using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Contracts;
using Backbone.Modules.Synchronization.Module.Features.SyncRuns.Shared;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Module.Features.SyncRuns.StartSyncRun;

internal static class Endpoint
{
    public static RouteGroupBuilder MapStartSyncRunEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Accepts<StartSyncRunRequestBody>(isOptional: true, "application/json", "application/*+json")
            .Produces<HttpResponseEnvelopeResult<StartSyncRunResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeResult<StartSyncRunResponse>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] StartSyncRunRequestBody? requestBody,
        [FromHeader(Name = "X-Supported-Datawallet-Version")] ushort? supportedDatawalletVersion,
        IIdentityStatusProvider identityStatusProvider, IUserContext userContext, IMediator mediator, CancellationToken cancellationToken)
    {
        if (requestBody == null)
            throw new BadHttpRequestException("The request body is required.");

        if (!await identityStatusProvider.IsActive(userContext.GetAddress(), cancellationToken))
            throw new ApplicationException(ApplicationErrors.SyncRuns.CannotStartSyncRunWhileIdentityIsToBeDeleted());

        var response = await mediator.Send(new StartSyncRunCommand
        {
            Type = requestBody.Type ?? SyncRunDTO.SyncRunType.ExternalEventSync,
            Duration = requestBody.Duration, SupportedDatawalletVersion = supportedDatawalletVersion ?? 0
        }, cancellationToken);
        return response.Status == StartSyncRunStatus.Created ? EnvelopeHttpResults.Created("", response) : EnvelopeHttpResults.Ok(response);
    }
}

public class StartSyncRunRequestBody
{
    public SyncRunDTO.SyncRunType? Type { get; set; }
    public ushort? Duration { get; set; }
}
