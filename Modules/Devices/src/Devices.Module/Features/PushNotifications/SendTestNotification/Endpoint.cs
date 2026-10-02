using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.PushNotifications.SendTestNotification;

internal static class Endpoint
{
    public static RouteGroupBuilder MapSendTestNotificationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("SendTestNotification", Handle)
            .Produces(StatusCodes.Status204NoContent)
            .ExcludeFromDescription();
        return group;
    }

    private static async Task<IResult> Handle([FromBody] object data, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new Command { Data = data }, cancellationToken);
        return Results.NoContent();
    }
}
