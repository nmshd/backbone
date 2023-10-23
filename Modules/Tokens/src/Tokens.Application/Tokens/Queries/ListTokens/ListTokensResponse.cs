using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Tokens.Application.Tokens.DTOs;

namespace Backbone.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensResponse : PagedResponse<TokenDTO>
{
    public ListTokensResponse(IEnumerable<TokenDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
