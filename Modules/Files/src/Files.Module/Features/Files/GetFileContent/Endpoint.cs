using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Mime;

namespace Backbone.Modules.Files.Module.Features.Files.GetFileContent;

internal static class Endpoint
{
    public const string ROUTE_NAME = "ConsumerApi.Files.Download";

    public static RouteGroupBuilder MapGetFileContentEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("{fileId}", Handle)
            .WithMetadata(new RouteNameMetadata(ROUTE_NAME))
            .Produces<byte[]>(StatusCodes.Status200OK, MediaTypeNames.Application.Octet)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status404NotFound);
        return group;
    }

    private static async Task<IResult> Handle([FromRoute] string fileId, IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new Query { Id = fileId }, cancellationToken);
        return Results.File(response.FileContent, MediaTypeNames.Application.Octet);
    }
}
