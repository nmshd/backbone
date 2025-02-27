using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.CancelDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAsSupport;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GetIdentityQueryDevices = Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity.GetIdentityQuery;
using GetIdentityQueryQuotas = Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity.GetIdentityQuery;

namespace Backbone.AdminApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class IdentitiesController : ApiControllerBase
{
    public IdentitiesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("{identityAddress}/Quotas")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<IndividualQuotaDTO>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<CreatedResult> CreateIndividualQuota([FromRoute] string identityAddress, [FromBody] CreateQuotaForIdentityRequest request, CancellationToken cancellationToken)
    {
        var createdIndividualQuotaDTO = await _mediator.Send(new CreateQuotaForIdentityCommand(identityAddress, request.MetricKey, request.Max, request.Period), cancellationToken);
        return Created(createdIndividualQuotaDTO);
    }

    [HttpDelete("{identityAddress}/Quotas/{individualQuotaId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteIndividualQuota([FromRoute] string identityAddress, [FromRoute] string individualQuotaId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteQuotaForIdentityCommand(identityAddress, individualQuotaId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{address}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<GetIdentityResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIdentityByAddress([FromRoute] string address, CancellationToken cancellationToken)
    {
        var identity = await _mediator.Send(new GetIdentityQueryDevices(address), cancellationToken);
        var quotas = await _mediator.Send(new GetIdentityQueryQuotas(address), cancellationToken);

        var response = new GetIdentityResponse
        {
            Address = identity.Address,
            ClientId = identity.ClientId,
            PublicKey = identity.PublicKey,
            TierId = identity.TierId,
            CreatedAt = identity.CreatedAt,
            IdentityVersion = identity.IdentityVersion,
            NumberOfDevices = identity.NumberOfDevices,
            Devices = identity.Devices,
            Quotas = quotas.Quotas
        };

        return Ok(response);
    }

    [HttpPut("{identityAddress}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIdentity([FromRoute] string identityAddress, [FromBody] UpdateIdentityRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateIdentityCommand { Address = identityAddress, TierId = request.TierId };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateIdentityResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIdentity(CreateIdentityRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateIdentityCommand
        {
            ClientId = request.ClientId,
            DevicePassword = request.DevicePassword,
            IdentityPublicKey = request.IdentityPublicKey,
            IdentityVersion = request.IdentityVersion,
            CommunicationLanguage = request.DeviceCommunicationLanguage,
            SignedChallenge = new SignedChallengeDTO
            {
                Challenge = request.SignedChallenge.Challenge,
                Signature = request.SignedChallenge.Signature
            }
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created(response);
    }

    [HttpPost("{address}/DeletionProcesses")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartDeletionProcess([FromRoute] string address, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new StartDeletionProcessAsSupportCommand(address), cancellationToken);
        return Created("", response);
    }

    [HttpGet("{identityAddress}/DeletionProcesses")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<GetDeletionProcessesAsSupportResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDeletionProcessesAsSupport([FromRoute] string identityAddress, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessesAsSupportQuery(identityAddress), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{identityAddress}/DeletionProcesses/AuditLogs")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<GetDeletionProcessesAuditLogsResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDeletionProcessesAuditLogs([FromRoute] string identityAddress, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessesAuditLogsQuery(identityAddress), cancellationToken);
        return Ok(response);
    }

    [HttpPut("{address}/DeletionProcesses/{deletionProcessId}/Cancel")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CancelDeletionAsSupportResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelDeletionProcessAsSupport([FromRoute] string address, [FromRoute] string deletionProcessId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelDeletionAsSupportCommand(address, deletionProcessId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{identityAddress}/DeletionProcesses/{deletionProcessId}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<IdentityDeletionProcessDetailsDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeletionProcessAsSupport([FromRoute] string identityAddress, [FromRoute] string deletionProcessId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessAsSupportQuery(identityAddress, deletionProcessId), cancellationToken);
        return Ok(response);
    }
}

public class CreateQuotaForIdentityRequest
{
    public required string MetricKey { get; set; }
    public required int Max { get; set; }
    public required QuotaPeriod Period { get; set; }
}

public class UpdateIdentityRequest
{
    public required string TierId { get; set; }
}

public class GetIdentityResponse
{
    public required string Address { get; set; }
    public required string? ClientId { get; set; }
    public required byte[] PublicKey { get; set; }
    public required string TierId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required byte IdentityVersion { get; set; }
    public required int NumberOfDevices { get; set; }
    public required IEnumerable<DeviceDTO> Devices { get; set; }
    public required IEnumerable<QuotaDTO> Quotas { get; set; }
}

public class CreateIdentityRequest
{
    public required string ClientId { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required string DeviceCommunicationLanguage { get; set; }
    public required byte IdentityVersion { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
}
