using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.Modules.Files.Module.Features.Files.Shared;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileMetadata;

internal static class Endpoint
{
    public static RouteGroupBuilder MapGetFileMetadataEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{fileId}/Metadata", Handle)
            .Produces<HttpResponseEnvelopeResult<FileMetadataDTO>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string fileId, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetFileMetadataQuery { Id = fileId }, cancellationToken);
        return EnvelopeHttpResults.Ok(response);
    }
}
