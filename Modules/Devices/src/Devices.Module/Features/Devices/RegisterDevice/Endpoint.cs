using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRegisterDeviceEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] RequestBody body, IMediator mediator, CancellationToken cancellationToken)
    {
        if (body.DevicePassword == null || body.SignedChallenge == null || body.SignedChallenge.Challenge == null || body.SignedChallenge.Signature == null)
            throw new BadHttpRequestException("Required request fields must not be null.");

        var command = new Command
        {
            CommunicationLanguage = body.CommunicationLanguage ?? CommunicationLanguage.DEFAULT_LANGUAGE.Value,
            SignedChallenge = body.SignedChallenge,
            DevicePassword = body.DevicePassword,
            IsBackupDevice = body.IsBackupDevice ?? false
        };
        var response = await mediator.Send(command, cancellationToken);
        return EnvelopeHttpResults.Created("", response);
    }
}

public class RequestBody
{
    public required string DevicePassword { get; set; }
    public string? CommunicationLanguage { get; set; }
    public required SignedChallengeDTO SignedChallenge { get; set; }
    public bool? IsBackupDevice { get; set; }
}
