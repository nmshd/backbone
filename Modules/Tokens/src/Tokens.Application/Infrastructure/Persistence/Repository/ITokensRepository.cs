using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokensRepository
{
    Task Add(Token token);

    Task<DbPaginationResult<Token>> FindTokens(IEnumerable<ListTokensQueryItem> queryItems, IdentityAddress activeIdentity, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<IEnumerable<Token>> FindTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false);

    Task<Token?> Find(TokenId tokenId, IdentityAddress? activeIdentity, CancellationToken cancellationToken, bool track = false);
    Task Update(IEnumerable<Token> tokens, CancellationToken cancellationToken);
    Task DeleteTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken);
    Task DeleteToken(Token token, CancellationToken cancellationToken);
}
