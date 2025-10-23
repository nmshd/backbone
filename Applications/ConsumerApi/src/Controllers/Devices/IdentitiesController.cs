using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.ConsumerApi.Versions;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.ChangeFeatureFlags;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;
using Backbone.Modules.Devices.Application.Identities.Queries.GetOwnIdentity;
using Backbone.Modules.Devices.Application.Identities.Queries.IsIdentityOfUserDeleted;
using Backbone.Modules.Devices.Application.Identities.Queries.ListFeatureFlags;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.OpenIddict;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.ConsumerApi.Controllers.Devices;

[V2]
[Route("api/v{v:apiVersion}/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class IdentitiesController : ApiControllerBase
{
    private readonly OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public IdentitiesController(IMediator mediator, OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager) : base(mediator)
    {
        _applicationManager = applicationManager;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateIdentityResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
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
            CommunicationLanguage = request.DeviceCommunicationLanguage ?? CommunicationLanguage.DEFAULT_LANGUAGE.Value,
            SignedChallenge = new SignedChallengeDTO
            {
                Challenge = request.SignedChallenge.Challenge,
                Signature = request.SignedChallenge.Signature
            }
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Created(response);
    }

    [HttpGet("Self")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<GetOwnIdentityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOwnIdentity(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOwnIdentityQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("IsDeleted")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<IsIdentityOfUserDeletedResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IsIdentityOfUserDeleted([FromQuery(Name = "username")] string username, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new IsIdentityOfUserDeletedQuery { Username = username }, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("Self/FeatureFlags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeFeatureFlags([FromBody] ChangeFeatureFlagsCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpGet("{identityAddress}/FeatureFlags")]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<ListFeatureFlagsResponse>), StatusCodes.Status200OK)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [ProducesError(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListFeatureFlags([FromRoute] string identityAddress, CancellationToken cancellationToken)
    {
        var request = new ListFeatureFlagsQuery { IdentityAddress = identityAddress };
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}

public class CreateIdentityRequest
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public string? DeviceCommunicationLanguage { get; set; }
    public required byte IdentityVersion { get; set; }
    public required CreateIdentityRequestSignedChallenge SignedChallenge { get; set; }
}

public class CreateIdentityRequestSignedChallenge
{
    public required string Challenge { get; set; }
    public required byte[] Signature { get; set; }
}
