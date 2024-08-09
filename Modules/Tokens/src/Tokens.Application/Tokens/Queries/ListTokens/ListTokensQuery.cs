using Backbone.BuildingBlocks.Application.Pagination;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensQuery : IRequest<ListTokensResponse>
{
    public ListTokensQuery(PaginationFilter paginationFilter, IEnumerable<string> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<string> Ids { get; set; }
}
