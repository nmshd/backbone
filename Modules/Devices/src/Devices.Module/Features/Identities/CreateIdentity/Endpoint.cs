using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.CreateIdentity;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCreateIdentityEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<CreateIdentityResponse>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound)
            .AllowAnonymous();
        return group;
    }

    private static async Task<IResult> Handle([FromBody] RequestBody request, IOAuthClientsRepository clients, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request.ClientId == null || request.ClientSecret == null || request.IdentityPublicKey == null || request.DevicePassword == null || request.SignedChallenge == null || request.SignedChallenge.Challenge == null || request.SignedChallenge.Signature == null)
            throw new BadHttpRequestException("Required request fields must not be null.");

        if (!await clients.ValidateSecret(request.ClientId, request.ClientSecret, cancellationToken))
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
        var response = await mediator.Send(command, cancellationToken);
        return EnvelopeHttpResults.Created("", response);
    }
}

public class RequestBody
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required byte[] IdentityPublicKey { get; set; }
    public required string DevicePassword { get; set; }
    public string? DeviceCommunicationLanguage { get; set; }
    public required byte IdentityVersion { get; set; }
    public required SignedChallengeC SignedChallenge { get; set; }

    public class SignedChallengeC
    {
        public required string Challenge { get; set; }
        public required byte[] Signature { get; set; }
    }
}
