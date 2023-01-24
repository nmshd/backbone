using Enmeshed.BuildingBlocks.Application.Pagination;
using Tokens.Application.Tokens.DTOs;

namespace Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensResponse : PagedResponse<TokenDTO>
{
    public ListTokensResponse(IEnumerable<TokenDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
