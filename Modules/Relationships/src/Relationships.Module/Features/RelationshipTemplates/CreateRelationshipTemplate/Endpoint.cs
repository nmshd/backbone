using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.CreateRelationshipTemplate;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCreateRelationshipTemplateEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<CreateRelationshipTemplateResponse>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] CreateRelationshipTemplateCommand request, HttpContext context, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request.Content == null)
            throw new BadHttpRequestException("Content is required.");

        var response = await mediator.Send(request, cancellationToken);
        return EnvelopeHttpResults.CreatedAtRoute("GetRelationshipTemplate", new { id = response.Id, v = context.Request.RouteValues["v"] }, response);
    }
}
