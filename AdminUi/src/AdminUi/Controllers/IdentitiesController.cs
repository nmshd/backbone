using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsSupport;
using Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;
using Backbone.Modules.Quotas.Application.DTOs;
using Backbone.Modules.Quotas.Application.Identities.Commands.CreateQuotaForIdentity;
using Backbone.Modules.Quotas.Application.Identities.Commands.DeleteQuotaForIdentity;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GetIdentityQueryDevices = Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity.GetIdentityQuery;
using GetIdentityQueryQuotas = Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity.GetIdentityQuery;
using GetIdentityResponseDevices = Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity.GetIdentityResponse;
using GetIdentityResponseQuotas = Backbone.Modules.Quotas.Application.Identities.Queries.GetIdentity.GetIdentityResponse;

namespace Backbone.AdminUi.Controllers;

[Route("api/v1/[controller]")]
[Authorize("ApiKey")]
public class IdentitiesController : ApiControllerBase
{
    public IdentitiesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("{identityAddress}/Quotas")]
    [ProducesResponseType(typeof(IndividualQuotaDTO), StatusCodes.Status201Created)]
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
    [ProducesResponseType(typeof(GetIdentityResponse), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIdentityByAddress([FromRoute] string address, CancellationToken cancellationToken)
    {
        var identity = await _mediator.Send<GetIdentityResponseDevices>(new GetIdentityQueryDevices(address), cancellationToken);
        var quotas = await _mediator.Send<GetIdentityResponseQuotas>(new GetIdentityQueryQuotas(address), cancellationToken);

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
    public async Task<IActionResult> UpdateIdentity([FromRoute] string identityAddress, [FromBody] UpdateIdentityTierRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateIdentityCommand() { Address = identityAddress, TierId = request.TierId };
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
            SignedChallenge = new SignedChallengeDTO
            {
                Challenge = request.SignedChallenge.Challenge,
                Signature = request.SignedChallenge.Signature
            },
            ShouldValidateChallenge = false
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created(response);
    }

    [HttpPost("{address}/DeletionProcesses")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartDeletionProcessAsSupport([FromRoute] string address, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new StartDeletionProcessAsSupportCommand(address), cancellationToken);
        return Created("", response);
    }
}

public class CreateQuotaForIdentityRequest
{
    public string MetricKey { get; set; }
    public int Max { get; set; }
    public QuotaPeriod Period { get; set; }
}

public class UpdateIdentityTierRequest
{
    public string TierId { get; set; }
}

public class GetIdentityResponse
{
    public string Address { get; set; }
    public string ClientId { get; set; }
    public byte[] PublicKey { get; set; }
    public string TierId { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte IdentityVersion { get; set; }
    public int NumberOfDevices { get; set; }
    public IEnumerable<DeviceDTO> Devices { get; set; }
    public IEnumerable<QuotaDTO> Quotas { get; set; }
}

public class CreateIdentityRequest
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public required byte IdentityVersion { get; set; }
    public required CreateIdentityRequestSignedChallenge SignedChallenge { get; set; }
}

public class CreateIdentityRequestSignedChallenge
{
    public required string Challenge { get; set; }
    public required byte[] Signature { get; set; }
}
