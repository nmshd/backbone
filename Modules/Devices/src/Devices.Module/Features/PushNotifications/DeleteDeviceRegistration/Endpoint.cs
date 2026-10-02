using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.DeleteDeviceRegistration;

internal static class Endpoint
{
    public static RouteGroupBuilder MapDeleteDeviceRegistrationEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("", Handle)
            .Produces(StatusCodes.Status204NoContent);
        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new Command(), cancellationToken);
        return Results.NoContent();
    }
}
