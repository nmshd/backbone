using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Quotas.Module.Features.Identities.ListQuotasForIdentity;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListQuotasForIdentityEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query(), cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
