using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Synchronization.Application;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetExternalEventsOfSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.Queries.GetSyncRunById;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("OpenIddict.Validation.AspNetCore")]
public class SyncRunsController : ApiControllerBase
{
    private readonly ApplicationOptions _options;
    private readonly IUserContext _userContext;
    private readonly IIdentitiesRepository _identitiesRepository;

    public SyncRunsController(IMediator mediator, IOptions<ApplicationOptions> options, IUserContext userContext, IIdentitiesRepository identitiesRepository) : base(mediator)
    {
        _options = options.Value;
        _userContext = userContext;
        _identitiesRepository = identitiesRepository;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<StartSyncRunResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartSyncRun(StartSyncRunRequestBody requestBody,
        [FromHeader(Name = "X-Supported-Datawallet-Version")]
        ushort supportedDatawalletVersion, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(_userContext.GetAddress(), CancellationToken.None) ?? throw new NotFoundException(nameof(Identity));

        identity.EnsureIdentityIsNotToBeDeleted();

        var response = await _mediator.Send(new StartSyncRunCommand(
            requestBody.Type ?? SyncRunDTO.SyncRunType.ExternalEventSync, requestBody.Duration,
            supportedDatawalletVersion), cancellationToken);

        if (response.Status == StartSyncRunStatus.Created)
            return Created(response);

        return Ok(response);
    }

    [HttpPut("{id}/FinalizeExternalEventSync")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FinalizeExternalEventSync([FromRoute] SyncRunId id,
        [FromBody] FinalizeExternalEventSyncRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new FinalizeExternalEventSyncSyncRunCommand(id,
            request.ExternalEventResults, request.DatawalletModifications), cancellationToken);

        return Ok(response);
    }

    [HttpPut("{id}/FinalizeDatawalletVersionUpgrade")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<FinalizeExternalEventSyncSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FinalizeDatawalletVersionUpgrade([FromRoute] SyncRunId id,
        [FromBody] FinalizeDatawalletVersionUpgradeRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new FinalizeDatawalletVersionUpgradeSyncRunCommand(id,
            request.NewDatawalletVersion, request.DatawalletModifications), cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id}/ExternalEvents")]
    [ProducesResponseType(typeof(PagedHttpResponseEnvelope<GetExternalEventsOfSyncRunResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExternalEventsOfSyncRun([FromRoute] SyncRunId id,
        [FromQuery] PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        paginationFilter.PageSize ??= _options.Pagination.DefaultPageSize;

        if (paginationFilter.PageSize > _options.Pagination.MaxPageSize)
            throw new ApplicationException(
                GenericApplicationErrors.Validation.InvalidPageSize(_options.Pagination.MaxPageSize));

        var response = await _mediator.Send(new GetExternalEventsOfSyncRunQuery(id, paginationFilter), cancellationToken);

        return Paged(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<SyncRunDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSyncRunById(SyncRunId id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetSyncRunByIdQuery(id), cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id}/RefreshExpirationTime")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<RefreshExpirationTimeOfSyncRunResponse>),
        StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefreshExpirationTimeOfSyncRun(SyncRunId id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RefreshExpirationTimeOfSyncRunCommand(id), cancellationToken);
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
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}

public class FinalizeExternalEventSyncRequest
{
    public List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> ExternalEventResults { get; set; } = [];
    public List<PushDatawalletModificationItem> DatawalletModifications { get; set; } = [];
}
