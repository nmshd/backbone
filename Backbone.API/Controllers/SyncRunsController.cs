using Backbone.API.Mvc;
using Backbone.API.Mvc.ControllerAttributes;
using Backbone.Modules.Synchronization.Application;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenIddict.Validation.AspNetCore;
using ApplicationException = Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class SyncRunsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;

    public SyncRunsController(IMediator mediator, IOptions<ApplicationOptions> options) : base(mediator)
    {
        _options = options.Value;
    }


    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<StartSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartSyncRun(StartSyncRunRequestBody requestBody,
        [FromHeader(Name = "X-Supported-Datawallet-Version")]
        ushort supportedDatawalletVersion)
    {
        var response = await _mediator.Send(new StartSyncRunCommand(
            requestBody.Type ?? SyncRunDTO.SyncRunType.ExternalEventSync, requestBody.Duration,
            supportedDatawalletVersion));

        if (response.Status == StartSyncRunStatus.Created)
            return Created(response);

        return Ok(response);
    }

    [HttpPut("{id}/Finalize")]
    [Obsolete("Use '/{id}/FinalizeExternalEventSync' instead.")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Finalize([FromRoute] SyncRunId id,
        [FromBody] OldFinalizeExternalEventSyncRequest request)
    {
        var modificationsWithVersion = request.DatawalletModifications.Select(m => new PushDatawalletModificationItem
        {
            DatawalletVersion = 0,
            Type = m.Type,
            Collection = m.Collection,
            EncryptedPayload = m.EncryptedPayload,
            ObjectIdentifier = m.ObjectIdentifier,
            PayloadCategory = m.PayloadCategory
        }).ToList();

        var response =
            await _mediator.Send(new FinalizeExternalEventSyncSyncRunCommand(id, request.ExternalEventResults,
                modificationsWithVersion));

        return Ok(response);
    }

    [HttpPut("{id}/FinalizeExternalEventSync")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FinalizeExternalEventSync([FromRoute] SyncRunId id,
        [FromBody] FinalizeExternalEventSyncRequest request)
    {
        var response = await _mediator.Send(new FinalizeExternalEventSyncSyncRunCommand(id,
            request.ExternalEventResults, request.DatawalletModifications));

        return Ok(response);
    }

    [HttpPut("{id}/FinalizeDatawalletVersionUpgrade")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FinalizeDatawalletVersionUpgrade([FromRoute] SyncRunId id,
        [FromBody] FinalizeDatawalletVersionUpgradeRequest request)
    {
        var response = await _mediator.Send(new FinalizeDatawalletVersionUpgradeSyncRunCommand(id,
            request.NewDatawalletVersion, request.DatawalletModifications));

        return Ok(response!);
    }

    [HttpGet("{id}/ExternalEvents")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<GetExternalEventsOfSyncRunResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExternalEventsOfSyncRun([FromRoute] SyncRunId id,
        [FromQuery] PaginationFilter paginationFilter)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(new GetExternalEventsOfSyncRunQuery(id, paginationFilter));

        return Paged(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<SyncRunDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSyncRunById(SyncRunId id)
    {
        var response = await _mediator.Send(new GetSyncRunByIdQuery(id));
        return Ok(response);
    }

    [HttpPut("{id}/RefreshExpirationTime")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RefreshExpirationTimeOfSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshExpirationTimeOfSyncRun(SyncRunId id)
    {
        var response = await _mediator.Send(new RefreshExpirationTimeOfSyncRunCommand(id));
        return Ok(response);
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
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = new();
}

public class FinalizeExternalEventSyncRequest
{
    public List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> ExternalEventResults { get; set; } = new();
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = new();
}

public class OldFinalizeExternalEventSyncRequest
{
    public List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> ExternalEventResults { get; set; } = new();
    public List<FinalizeExternalEventSyncRequestDatawalletModification> DatawalletModifications { get; set; } = new();

    public class FinalizeExternalEventSyncRequestDatawalletModification
    {
        public string ObjectIdentifier { get; set; }
        public string PayloadCategory { get; set; }
        public string Collection { get; set; }
        public DatawalletModificationDTO.DatawalletModificationType Type { get; set; }
        public byte[] EncryptedPayload { get; set; }
        public ushort DatawalletVersion { get; set; }
    }
}