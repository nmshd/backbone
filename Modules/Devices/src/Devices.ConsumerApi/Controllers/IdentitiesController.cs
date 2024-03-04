using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.ApproveDeletionProcess;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcessAsOwner;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcess;
using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcesses;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Modules.Devices.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class IdentitiesController : ApiControllerBase
{
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public IdentitiesController(
        IMediator mediator,
        OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager) : base(mediator)
    {
        _applicationManager = applicationManager;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateIdentityResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateIdentity(CreateIdentityRequest request, CancellationToken cancellationToken)
    {
        var client = await _applicationManager.FindByClientIdAsync(request.ClientId, cancellationToken);

        if (client == null || !await _applicationManager.ValidateClientSecretAsync(client, request.ClientSecret, cancellationToken))
            throw new OperationFailedException(GenericApplicationErrors.Unauthorized());

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
            }
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created(response);
    }

    [HttpPost("Self/DeletionProcesses")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartDeletionProcess(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new StartDeletionProcessAsOwnerCommand(), cancellationToken);
        return Created("", response);
    }

    [HttpPut("Self/DeletionProcesses/{id}/Approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveDeletionProcess([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new ApproveDeletionProcessCommand(id), cancellationToken);
        return Ok(response);
    }

    [HttpGet("Self/DeletionProcesses/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeletionProcess([FromRoute] string id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessQuery { Id = id }, cancellationToken);
        return Ok(response);
    }

    [HttpGet("Self/DeletionProcesses")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDeletionProcesses(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetDeletionProcessesQuery(), cancellationToken);
        return Ok(response);
    }
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
