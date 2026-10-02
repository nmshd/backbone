using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;
using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Files.Module.Features.Files.ListFileMetadata;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListFileMetadataEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .Produces<PagedHttpResponseEnvelope<Response>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromQuery(Name = "PageNumber")] int? pageNumber, [FromQuery(Name = "PageSize")] int? pageSize,
        [FromQuery] string[]? ids, IMediator mediator, IOptions<ApplicationConfiguration> options, CancellationToken cancellationToken)
    {
        var paginationFilter = new PaginationFilter { PageNumber = pageNumber ?? 1, PageSize = pageSize ?? options.Value.Pagination.DefaultPageSize };
        if (paginationFilter.PageSize > options.Value.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(options.Value.Pagination.MaxPageSize));

        var response = await mediator.Send(new Query { PaginationFilter = paginationFilter, Ids = ids ?? [] }, cancellationToken);
        return EnvelopeHttpResults.Paged(response);
    }
}
