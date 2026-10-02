using Backbone.BuildingBlocks.API;
using Backbone.BuildingBlocks.API.MinimalApi;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Pagination;
using Microsoft.Extensions.Options;
using ApplicationException = Backbone.BuildingBlocks.Application.Abstractions.Exceptions.ApplicationException;

namespace Backbone.Modules.Synchronization.Module.Features.Datawallets.ListModifications;

internal static class Endpoint
{
    public static RouteGroupBuilder MapListModificationsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("Modifications", Handle)
            .Produces<PagedHttpResponseEnvelope<ListModificationsResponse>>(StatusCodes.Status200OK)
            .Produces<HttpResponseEnvelopeError>(StatusCodes.Status400BadRequest);
        return group;
    }

    private static async Task<IResult> Handle([FromQuery(Name = "PageNumber")] int? pageNumber, [FromQuery(Name = "PageSize")] int? pageSize,
        [FromQuery] int? localIndex, [FromHeader(Name = "X-Supported-Datawallet-Version")] ushort? supportedDatawalletVersion,
        IMediator mediator, IOptions<ApplicationConfiguration> options, CancellationToken cancellationToken)
    {
        var paginationFilter = new PaginationFilter { PageNumber = pageNumber ?? 1, PageSize = pageSize };
        if (paginationFilter.PageSize > options.Value.Pagination.MaxPageSize)
            throw new ApplicationException(GenericApplicationErrors.Validation.InvalidPageSize(options.Value.Pagination.MaxPageSize));
        paginationFilter.PageSize ??= options.Value.Pagination.DefaultPageSize;

        var response = await mediator.Send(new ListModificationsQuery
        {
            PaginationFilter = paginationFilter, LocalIndex = localIndex, SupportedDatawalletVersion = supportedDatawalletVersion ?? 0
        }, cancellationToken);
        return EnvelopeHttpResults.Paged(response);
    }
}
