using Enmeshed.BuildingBlocks.Application.Pagination;
using MediatR;
using Tokens.Domain.Entities;

namespace Tokens.Application.Tokens.Queries.ListTokens;

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
