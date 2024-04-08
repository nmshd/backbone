using System.Collections;
using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Backbone.BuildingBlocks.Application.Extensions;

public static class IQueryablePaginationExtensions
{
    public static async Task<DbPaginationResult<T>> OrderAndPaginate<T, TOrderProperty>(this IQueryable<T> query,
        Expression<Func<T, TOrderProperty>> orderBy, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        var items = await query.OrderBy(orderBy).Paged(paginationFilter).ToListAsync(cancellationToken);
        var totalNumberOfItems = await CalculateTotalNumberOfItems(paginationFilter, items, query);

        return new DbPaginationResult<T>(items, totalNumberOfItems);
    }

    private static async Task<int> CalculateTotalNumberOfItems<T>(PaginationFilter paginationFilter,
        ICollection items, IQueryable<T> query)
    {
        int totalNumberOfItems;
        if (paginationFilter.PageSize == null)
        {
            totalNumberOfItems = items.Count;
        }
        else
        {
            if (items.Count < paginationFilter.PageSize)
                totalNumberOfItems = (paginationFilter.PageNumber - 1) * paginationFilter.PageSize.Value + items.Count;
            else
                totalNumberOfItems = await query.CountAsync();
        }

        return totalNumberOfItems;
    }

    public static IQueryable<T> Paged<T>(this IQueryable<T> query, PaginationFilter paginationFilter)
    {
        if (paginationFilter == null) throw new Exception("A pagination filter has to be provided.");

        if (paginationFilter.PageSize != null)
            query = query
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize.Value)
                .Take(paginationFilter.PageSize.Value);

        return query;
    }
}
