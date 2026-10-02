using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Module.Features.Tokens.Shared;

namespace Backbone.Modules.Tokens.Module.Features.Tokens.Shared;

[Backbone.BuildingBlocks.Application.Abstractions.JsonSchemaName("ListTokensResponse")]
public class Response(DbPaginationResult<Token> dbPaginationResult, PaginationFilter previousPaginationFilter)
    : PagedResponse<TokenDTO>(dbPaginationResult.ItemsOnPage.Select(t => new TokenDTO(t)), previousPaginationFilter, dbPaginationResult.TotalNumberOfItems);
