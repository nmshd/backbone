using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Devices.ChangePassword;

internal static class Endpoint
{
    public static RouteGroupBuilder MapChangePasswordEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("Self/Password", Handle)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] Command request, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request.OldPassword == null || request.NewPassword == null)
            throw new BadHttpRequestException("Required request fields must not be null.");

        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
