using Backbone.API;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation.AspNetCore;

namespace Devices.ConsumerApi.Controllers;

[Route("api/v1/[controller]")]
[Authorize(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class IdentitiesController : ApiControllerBase
{
    private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _applicationManager;

    public IdentitiesController(
        IMediator mediator,
        OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager) : base(mediator)
    {
        _applicationManager = applicationManager;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateIdentityResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateIdentity(CreateIdentityRequest request)
    {
        var client = await _applicationManager.FindByClientIdAsync(request.ClientId);

        if (client == null || !await _applicationManager.ValidateClientSecretAsync(client, request.ClientSecret))
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

        var response = await _mediator.Send(command);

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
