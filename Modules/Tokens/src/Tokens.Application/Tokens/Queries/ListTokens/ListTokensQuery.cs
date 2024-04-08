using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Domain.Entities;
using MediatR;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensQuery : IRequest<ListTokensResponse>
{
    public ListTokensQuery(PaginationFilter paginationFilter, IEnumerable<TokenId> ids)
    {
        PaginationFilter = paginationFilter;
        Ids = ids;
    }

    public PaginationFilter PaginationFilter { get; set; }
    public IEnumerable<TokenId> Ids { get; set; }
}
