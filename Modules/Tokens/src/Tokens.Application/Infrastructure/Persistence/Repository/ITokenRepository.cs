using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokenRepository
{
    Task<DbPaginationResult<Token>> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter);
    Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter);
    Task<IdentityAddress> GetOwner(TokenId tokenId);
    Task<IEnumerable<TokenId>> GetAllTokenIds(bool includeExpired = false);
    Task Add(Token token);
    Task Remove(Token token);
    Task AddRange(IEnumerable<Token> tokens);
    Task RemoveRange(IEnumerable<Token> tokens);
    Task<Token> Find(TokenId tokenId);
}
