using Backbone.BuildingBlocks.API.MinimalApi;
using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Relationships.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Relationships.Module.Features.RelationshipTemplates.ListRelationshipTemplates;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListRelationshipTemplatesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("", Handle)
            .Produces<PagedHttpResponseEnvelope<ListRelationshipTemplatesResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromQuery(Name = "PageNumber")] int? pageNumber, [FromQuery(Name = "PageSize")] int? pageSize,
        [FromQuery] string[]? ids, HttpRequest request, IMediator mediator, IOptions<ApplicationConfiguration> options, CancellationToken cancellationToken)
    {
        var queryItems = TemplateQueryItems.Read(request.Query);
        if (queryItems.Count == 0)
            queryItems = (ids ?? []).Select(id => new ListRelationshipTemplatesQueryItem { Id = id }).ToList();
        var paginationFilter = new PaginationFilter { PageNumber = pageNumber ?? 1, PageSize = pageSize ?? options.Value.Pagination.DefaultPageSize };
        if (paginationFilter.PageSize > options.Value.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(options.Value.Pagination.MaxPageSize));

        var response = await mediator.Send(new ListRelationshipTemplatesQuery { PaginationFilter = paginationFilter, QueryItems = queryItems }, cancellationToken);
        return EnvelopeHttpResults.Paged(response);
    }
}
