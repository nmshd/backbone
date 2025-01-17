using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.Queries.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensQuery : IRequest<ListTokensResponse>
{
    public ListTokensQuery(PaginationFilter paginationFilter, IEnumerable<ListTokensQueryItem>? queries)
    {
        PaginationFilter = paginationFilter;
        QueryItems = queries == null ? [] : queries.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<ListTokensQueryItem> QueryItems { get; set; }
}

public class ListTokensQueryItem
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
