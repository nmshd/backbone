using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensQuery : IRequest<ListTokensResponse>
{
    public ListTokensQuery(PaginationFilter paginationFilter, IEnumerable<TokenQueryItem>? queries)
    {
        PaginationFilter = paginationFilter;
        QueryItems = queries == null ? [] : queries.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<TokenQueryItem> QueryItems { get; set; }
}

public class TokenQueryItem
{
    public required string Id { get; set; }
    public byte[]? Password { get; set; }
}
