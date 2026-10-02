using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.Modules.Relationships.Module.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.GetRelationshipTemplate;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetRelationshipTemplateEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{id}", Handle)
            .Produces<HttpResponseEnvelopeResult<RelationshipTemplateDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound)
            .WithMetadata(new RouteNameMetadata("GetRelationshipTemplate"));
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string id, [FromQuery] Base64QueryValue? password, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query { Id = id, Password = password?.Value }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
