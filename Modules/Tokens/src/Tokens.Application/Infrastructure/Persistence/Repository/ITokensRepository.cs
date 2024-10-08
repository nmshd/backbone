using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokensRepository
{
    Task Add(Token token);
    Task<Token?> Find(TokenId tokenId, IdentityAddress? activeIdentity);
    Task<DbPaginationResult<Token>> FindAllWithIds(IdentityAddress activeIdentity, IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken);
    Task DeleteTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken);
}
