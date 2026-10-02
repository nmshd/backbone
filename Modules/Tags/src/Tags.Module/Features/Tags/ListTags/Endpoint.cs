using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Tags.Module.Features.Tags.ListTags;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListTagsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .AllowAnonymous()
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK);

        return group;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query(), cancellationToken);
        return EnvelopeHttpResults.Ok(response).WithHttpCaching();
    }
}
