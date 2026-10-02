using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Announcements.Module.Features.Announcements.ListAnnouncementsForActiveIdentityInLanguage;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListAnnouncementsInLanguageEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .Produces<HttpResponseEnvelopeResult<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);

        return group;
    }

    private static async Task<IResult> Handle([FromQuery] string language, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query { Language = language }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
