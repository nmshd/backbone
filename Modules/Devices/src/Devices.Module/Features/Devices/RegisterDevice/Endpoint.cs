using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Devices.RegisterDevice;

internal static class Endpoint
{
    public static RouteGroupBuilder MapRegisterDeviceEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<RegisterDeviceResponse>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] RegisterDeviceRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request.DevicePassword == null || request.SignedChallenge == null || request.SignedChallenge.Challenge == null || request.SignedChallenge.Signature == null)
            throw new BadHttpRequestException("Required request fields must not be null.");

        var command = new RegisterDeviceCommand
        {
            CommunicationLanguage = request.CommunicationLanguage ?? CommunicationLanguage.DEFAULT_LANGUAGE.Value,
            SignedChallenge = request.SignedChallenge,
            DevicePassword = request.DevicePassword,
            IsBackupDevice = request.IsBackupDevice ?? false
        };
        var response = await mediator.Send(command, cancellationToken);
        return EnvelopeHttpResults.Created("", response);
    }
}
