using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Devices.Application.Devices.DTOs;
using Backbone.Devices.Application.Identities.Commands.CreateIdentity;
using Backbone.Devices.Infrastructure.OpenIddict;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using OpenIddict.Validation.AspNetCore;

namespace Backbone.Devices.ConsumerApi.Controllers;

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

        return Created("", response);
    }
}

public class CreateIdentityRequest
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public byte[] IdentityPublicKey { get; set; }
    public string DevicePassword { get; set; }
    public byte IdentityVersion { get; set; }
    public CreateIdentityRequestSignedChallenge SignedChallenge { get; set; }
}

public class CreateIdentityRequestSignedChallenge
{
    public string Challenge { get; set; }
    public byte[] Signature { get; set; }
}
