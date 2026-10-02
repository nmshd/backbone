using Backbone.AdminApi.Versions;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using Backbone.Modules.Devices.Module.Features.Shared;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Module.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreateIdentity = Backbone.Modules.Devices.Module.Features.Identities.CreateIdentity;
using CreateQuotaForIdentity = Backbone.Modules.Quotas.Module.Features.Identities.CreateQuotaForIdentity;
using DeleteQuotaForIdentity = Backbone.Modules.Quotas.Module.Features.Identities.DeleteQuotaForIdentity;
using GetDeletionProcessAsSupport = Backbone.Modules.Devices.Module.Features.Identities.GetDeletionProcessAsSupport;
using GetIdentityQueryDevices = Backbone.Modules.Devices.Module.Features.Identities.GetIdentity.Query;
using GetIdentityQueryQuotas = Backbone.Modules.Quotas.Module.Features.Identities.GetIdentity.Query;
using ListDeletionProcessesAsSupport = Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsSupport;
using ListDeletionProcessesAuditLogs = Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAuditLogs;
using UpdateIdentity = Backbone.Modules.Devices.Module.Features.Identities.UpdateIdentity;

namespace Backbone.AdminApi.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
[Authorize("ApiKey")]
[V1]
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
        var createdIndividualQuotaDTO =
            await _mediator.Send(new CreateQuotaForIdentity.Command { IdentityAddress = identityAddress, MetricKey = request.MetricKey, Max = request.Max, Period = request.Period }, cancellationToken);
        return Created(createdIndividualQuotaDTO);
    }

    [HttpDelete("{identityAddress}/Quotas/{individualQuotaId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesError(StatusCodes.Status404NotFound)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteIndividualQuota([FromRoute] string identityAddress, [FromRoute] string individualQuotaId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteQuotaForIdentity.Command { IdentityAddress = identityAddress, IndividualQuotaId = individualQuotaId }, cancellationToken);
        return NoContent();
    }

    [HttpGet("{address}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<GetIdentityResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIdentityByAddress([FromRoute] string address, CancellationToken cancellationToken)
    {
        var identity = await _mediator.Send(new GetIdentityQueryDevices { Address = address }, cancellationToken);
        var quotas = await _mediator.Send(new GetIdentityQueryQuotas { Address = address }, cancellationToken);

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
        var command = new UpdateIdentity.Command { Address = identityAddress, TierId = request.TierId };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateIdentity.Response>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIdentity(CreateIdentityRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateIdentity.Command
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

    [HttpGet("{identityAddress}/DeletionProcesses")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListDeletionProcessesAsSupport.Response>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListDeletionProcessesAsSupport([FromRoute] string identityAddress, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListDeletionProcessesAsSupport.Query { IdentityAddress = identityAddress }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{identityAddress}/DeletionProcesses/AuditLogs")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListDeletionProcessesAuditLogs.Response>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListDeletionProcessesAuditLogs([FromRoute] string identityAddress, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ListDeletionProcessesAuditLogs.Query { IdentityAddress = identityAddress }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{identityAddress}/DeletionProcesses/{deletionProcessId}")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<IdentityDeletionProcessDetailsDTO>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeletionProcessAsSupport([FromRoute] string identityAddress, [FromRoute] string deletionProcessId, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessAsSupport.Query { IdentityAddress = identityAddress, DeletionProcessId = deletionProcessId }, cancellationToken);
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
