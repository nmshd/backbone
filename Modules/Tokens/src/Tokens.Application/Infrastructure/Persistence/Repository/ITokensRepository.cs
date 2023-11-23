using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokensRepository
{
    Task Add(Token token);
    Task<Token> Find(TokenId tokenId);
    Task<DbPaginationResult<Token>> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task DeleteAllOfOwner(string identityAddress, CancellationToken cancellationToken);
}
