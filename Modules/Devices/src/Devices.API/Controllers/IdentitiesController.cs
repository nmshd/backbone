using System.Security.Cryptography;
using System.Text;
using Backbone.Modules.Devices.Application.Devices.DTOs;
using Backbone.Modules.Devices.Application.Identities.CreateIdentity;
using Enmeshed.BuildingBlocks.API;
using Enmeshed.BuildingBlocks.API.Mvc;
using Enmeshed.BuildingBlocks.API.Mvc.ControllerAttributes;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backbone.Modules.Devices.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize(IdentityServerConstants.LocalApi.PolicyName)]
public class IdentitiesController : ApiControllerBase
{
    private readonly IClientStore _clientStore;

    public IdentitiesController(IMediator mediator, IClientStore clientStore) : base(mediator)
    {
        _clientStore = clientStore;
    }

    [HttpPost]
    [ProducesResponseType(typeof(HttpResponseEnvelopeResult<CreateIdentityResponse>), StatusCodes.Status201Created)]
    [ProducesError(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateIdentity(CreateIdentityRequest request)
    {
        var client = (await _clientStore.FindClientByIdAsync(request.ClientId));

        if (client == null || !IsClientSecretCorrect(request, client))
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

    private static bool IsClientSecretCorrect(CreateIdentityRequest request, Client client)
    {
        if (request.ClientSecret.IsNullOrEmpty())
            return false;

        var clientSecretHash = HashClientSecret(request);

        var clientSecretMatches = client.ClientSecrets.Any(s => s.Value == clientSecretHash);

        return clientSecretMatches;
    }

    private static string HashClientSecret(CreateIdentityRequest request)
    {
        using var hasher = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(request.ClientSecret);
        var hash = Convert.ToBase64String(hasher.ComputeHash(bytes));
        return hash;
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
