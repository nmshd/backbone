using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Enmeshed.BuildingBlocks.Application.Pagination;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensResponse : PagedResponse<TokenDTO>
{
    public ListTokensResponse(IEnumerable<TokenDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
}
