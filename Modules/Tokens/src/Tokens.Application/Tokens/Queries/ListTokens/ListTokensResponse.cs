using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensResponse : PagedResponse<TokenDTO>
{
    public ListTokensResponse(IEnumerable<TokenDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
