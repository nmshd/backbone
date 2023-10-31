using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.Mvc;
using Backbone.BuildingBlocks.API.Mvc.ControllerAttributes;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;
using Backbone.Modules.Devices.Application.Identities.Commands.StartDeletionProcess;
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
    private readonly IUserContext _userContext;

    public IdentitiesController(
        IMediator mediator,
        OpenIddictApplicationManager<CustomOpenIddictEntityFrameworkCoreApplication> applicationManager, IUserContext userContext) : base(mediator)
    {
        _applicationManager = applicationManager;
        _userContext = userContext;
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

    [HttpPost("Self/DeletionProcess")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartDeletionProcess(CancellationToken cancellationToken)
    {
        await _mediator.Send(new StartDeletionProcessCommand(_userContext.GetAddress()), cancellationToken);
        return new CreatedResult("", null);
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
