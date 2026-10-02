using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Relationships.Module.Features.PublicRelationshipTemplateReferences.ListPublicRelationshipTemplateReferences;

internal static class Endpoint
{
    public static IEndpointRouteBuilder MapListPublicRelationshipTemplateReferencesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/poc/PublicRelationshipTemplateReferences", Handle)
            .RequireAuthorization("OpenIddict.Validation.AspNetCore")
            .ExcludeFromDescription()
            .WithMinimalApiErrorHandling();
        return endpoints;
    }

    private static async Task<IResult> Handle(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query(), cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
