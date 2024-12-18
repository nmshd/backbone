using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.AdminApi.Queries.GetAllTokens;

public class GetTokensResponse : PagedResponse<Token>
{
    public GetTokensResponse(DbPaginationResult<Token> dbPaginationResult, PaginationFilter previousPaginationFilter) : base(dbPaginationResult.ItemsOnPage,
        previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
    {
    }
}
