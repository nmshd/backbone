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
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.ListExternalEventsOfSyncRun;
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
    private readonly ApplicationConfiguration _configuration;
    private readonly IUserContext _userContext;

    public SyncRunsController(IMediator mediator, IOptions<ApplicationConfiguration> options, IUserContext userContext) : base(mediator)
    {
        _configuration = options.Value;
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
        var identityResponse = await _mediator.Send(new GetIdentityQuery { Address = _userContext.GetAddress() }, cancellationToken);
        EnsureIdentityIsActive(identityResponse);

        var response = await _mediator.Send(
            new StartSyncRunCommand { Type = requestBody.Type ?? SyncRunDTO.SyncRunType.ExternalEventSync, Duration = requestBody.Duration, SupportedDatawalletVersion = supportedDatawalletVersion },
            cancellationToken);

        return response.Status == StartSyncRunStatus.Created ? Created(response) : Ok(response);
    }

    [HttpPut("{id}/FinalizeExternalEventSync")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FinalizeExternalEventSync([FromRoute] string id,
        [FromBody] FinalizeExternalEventSyncRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = id, ExternalEventResults = request.ExternalEventResults, DatawalletModifications = request.DatawalletModifications },
            cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id}/FinalizeDatawalletVersionUpgrade")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FinalizeDatawalletVersionUpgrade([FromRoute] string id, [FromBody] FinalizeDatawalletVersionUpgradeRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new FinalizeDatawalletVersionUpgradeSyncRunCommand { SyncRunId = id, NewDatawalletVersion = request.NewDatawalletVersion, DatawalletModifications = request.DatawalletModifications },
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id}/ExternalEvents")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<ListExternalEventsOfSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListExternalEventsOfSyncRun([FromRoute] string id,
        [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _configuration.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _configuration.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(_configuration.Pagination.MaxPageSize));

        var response = await _mediator.Send(new ListExternalEventsOfSyncRunQuery { SyncRunId = id, PaginationFilter = paginationFilter }, cancellationToken);

        return Paged(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<SyncRunDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSyncRunById(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSyncRunByIdQuery { SyncRunId = id }, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/RefreshExpirationTime")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RefreshExpirationTimeOfSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshExpirationTimeOfSyncRun(string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RefreshExpirationTimeOfSyncRunCommand { SyncRunId = id }, cancellationToken);
        return Ok(response);
    }

    private static void EnsureIdentityIsActive(GetIdentityResponse identityResponse)
    {
        if (identityResponse.Status is not IdentityStatus.Active)
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
