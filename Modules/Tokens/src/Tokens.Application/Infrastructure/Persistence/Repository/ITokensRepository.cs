using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokensRepository
{
    Task Add(Token token);
    Task<Token> Find(TokenId tokenId);
    Task<DbPaginationResult<Token>> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter);
    Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter);
    Task<IEnumerable<TokenId>> GetAllTokenIds(bool includeExpired = false);
}
