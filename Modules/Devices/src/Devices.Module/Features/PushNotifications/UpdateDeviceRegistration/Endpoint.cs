using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.UpdateDeviceRegistration;

internal static class Endpoint
{
    public static RouteGroupBuilder MapUpdateDeviceRegistrationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("", Handle)
            .Produces<HttpResponseEnvelopeResult<UpdateDeviceRegistrationResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] UpdateDeviceRegistrationCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request.Platform == null || request.Handle == null || request.AppId == null)
            throw new BadHttpRequestException("Required request fields must not be null.");

        var response = await mediator.Send(request, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
