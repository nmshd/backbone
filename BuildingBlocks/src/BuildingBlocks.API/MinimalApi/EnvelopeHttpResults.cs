using Backbone.BuildingBlocks.Application.Pagination;
using Microsoft.AspNetCore.Http;

namespace Backbone.BuildingBlocks.API.MinimalApi;

public static class EnvelopeHttpResults
{
    public static IResult Ok<T>(T result) => Results.Ok(HttpResponseEnvelope.CreateSuccess(result));

    public static IResult Created<T>(string uri, T result) => Results.Created(uri, HttpResponseEnvelope.CreateSuccess(result));

    public static IResult CreatedAtRoute<T>(string routeName, object routeValues, T result) =>
        Results.CreatedAtRoute(routeName, routeValues, HttpResponseEnvelope.CreateSuccess(result));

    public static IResult Paged<T>(PagedResponse<T> response)
    {
        if (response.Pagination.TotalPages <= 1)
            return Ok<IEnumerable<T>>(response);

        return Results.Ok(new PagedHttpResponseEnvelope<T>(response, new PagedHttpResponseEnvelopePaginationData
        {
            TotalRecords = response.Pagination.TotalRecords,
            PageNumber = response.Pagination.PageNumber,
            TotalPages = response.Pagination.TotalPages,
            PageSize = response.Pagination.PageSize
        }));
    }
}
