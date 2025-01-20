using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.Queries.Shared;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensQuery : IRequest<ListTokensResponse>
{
    public ListTokensQuery(PaginationFilter paginationFilter, IEnumerable<string>? queries)
    {
        PaginationFilter = paginationFilter;
        QueryItems = queries == null ? [] : queries.ToList();
    }

    public PaginationFilter PaginationFilter { get; set; }
    public List<string> QueryItems { get; set; }
}
