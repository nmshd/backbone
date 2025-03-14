using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Synchronization.Application;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.ConsumerApi.Controllers.Synchronization;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class SyncRunsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;
    private readonly IUserContext _userContext;

    public SyncRunsController(IMediator mediator, IOptions<ApplicationOptions> options, IUserContext userContext) : base(mediator)
    {
        _options = options.Value;
        _userContext = userContext;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<StartSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<StartSyncRunResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartSyncRun(StartSyncRunRequestBody requestBody,
        [FromHeader(Name = "X-Supported-Datawallet-Version")]
        ushort supportedDatawalletVersion, CancellationToken cancellationToken)
    {
        var identityResponse = await _mediator.Send(new GetIdentityQuery(_userContext.GetAddress()), cancellationToken);
        EnsureIdentityIsNotToBeDeleted(identityResponse);

        var response = await _mediator.Send(new StartSyncRunCommand(requestBody.Type ?? SyncRunDTO.SyncRunType.ExternalEventSync, requestBody.Duration, supportedDatawalletVersion), cancellationToken);

        return response.Status == StartSyncRunStatus.Created ? Created(response) : Ok(response);
    }

    [HttpPut("{id}/FinalizeExternalEventSync")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FinalizeExternalEventSync([FromRoute] string id,
        [FromBody] FinalizeExternalEventSyncRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new FinalizeExternalEventSyncSyncRunCommand(id,
            request.ExternalEventResults, request.DatawalletModifications), cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id}/FinalizeDatawalletVersionUpgrade")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FinalizeDatawalletVersionUpgrade([FromRoute] string id, [FromBody] FinalizeDatawalletVersionUpgradeRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new FinalizeDatawalletVersionUpgradeSyncRunCommand(id, request.NewDatawalletVersion, request.DatawalletModifications), cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id}/ExternalEvents")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<GetExternalEventsOfSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExternalEventsOfSyncRun([FromRoute] string id,
        [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(new GetExternalEventsOfSyncRunQuery(id, paginationFilter), cancellationToken);

        return Paged(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<SyncRunDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSyncRunById(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSyncRunByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/RefreshExpirationTime")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RefreshExpirationTimeOfSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshExpirationTimeOfSyncRun(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RefreshExpirationTimeOfSyncRunCommand(id), cancellationToken);
        return Ok(response);
    }

    private static void EnsureIdentityIsNotToBeDeleted(GetIdentityResponse identityResponse)
    {
        if (identityResponse.Status is IdentityStatus.ToBeDeleted)
            throw new ApplicationException(ApplicationErrors.SyncRuns.CannotStartSyncRunWhileIdentityIsToBeDeleted());
    }
}

public class StartSyncRunRequestBody
{
    public SyncRunDTO.SyncRunType? Type { get; set; }

    public ushort? Duration { get; set; }
}

public class FinalizeDatawalletVersionUpgradeRequest
{
    public ushort NewDatawalletVersion { get; set; }
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}

public class FinalizeExternalEventSyncRequest
{
    public List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> ExternalEventResults { get; set; } = [];
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}
