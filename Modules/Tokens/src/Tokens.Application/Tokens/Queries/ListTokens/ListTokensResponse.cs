using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;

public class ListTokensResponse : PagedResponse<TokenDTO>
{
    public ListTokensResponse(DbPaginationResult<Token> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(dbPaginationResult.ItemsOnPage.Select(t => new TokenDTO(t)),
        previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
