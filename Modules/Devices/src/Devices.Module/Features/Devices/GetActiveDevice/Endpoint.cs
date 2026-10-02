using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Devices.Module.Features.Devices.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Devices.GetActiveDevice;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetActiveDeviceEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("Self", Handle)
            .Produces<HttpResponseEnvelopeResult<DeviceDTO>>(StatusCodes.Status200OK);
        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetActiveDeviceQuery(), cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
