using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Devices.Module.Features.Identities.ListDeletionProcessesAsOwner;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListDeletionProcessesAsOwnerEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .Produces<HttpResponseEnvelopeResult<ListDeletionProcessesAsOwnerResponse>>(StatusCodes.Status200OK);
        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ListDeletionProcessesAsOwnerQuery(), cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
