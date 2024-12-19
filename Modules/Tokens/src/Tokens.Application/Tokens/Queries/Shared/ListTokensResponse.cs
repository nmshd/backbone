using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Application.Tokens.DTOs;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Tokens.Queries.Shared;

public class ListTokensResponse(DbPaginationResult<Token> dbPaginationResult, PaginationFilter previousPaginationFilter)
    : PagedResponse<TokenDTO>(dbPaginationResult.ItemsOnPage.Select(t => new TokenDTO(t)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems)
{
}
