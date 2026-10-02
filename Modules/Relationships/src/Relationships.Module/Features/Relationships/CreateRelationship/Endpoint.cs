using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Devices.Contracts;
using Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.GetRelationshipTemplate;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Relationships.Module.Features.Relationships.CreateRelationship;

internal static class Endpoint
{
    public static RouteGroupBuilder MapCreateRelationshipEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("", Handle)
            .Produces<HttpResponseEnvelopeResult<CreateRelationshipResponse>>(StatusCodes.Status201Created)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromBody] CreateRelationshipCommand request, IIdentityStatusProvider identities, IMediator mediator, CancellationToken cancellationToken)
    {
        if (request.RelationshipTemplateId == null)
            throw new BadHttpRequestException("RelationshipTemplateId is required.");

        var template = await mediator.Send(new GetRelationshipTemplateQuery { Id = request.RelationshipTemplateId }, cancellationToken);
        if (!await identities.IsActive(template.CreatedBy, cancellationToken))
            throw new ApplicationException(ApplicationErrors.Relationship.PeerIsToBeDeleted());

        var response = await mediator.Send(request, cancellationToken);
        return EnvelopeHttpResults.Created("", response);
    }
}
