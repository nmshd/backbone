using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokensRepository
{
    Task Add(Token token);

    Task<DbPaginationResult<Token>> FindTokensAllocatedOrCreatedBy(IEnumerable<string> ids, IdentityAddress activeIdentity, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<IEnumerable<Token>> FindTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false);

    Task<Token?> Find(TokenId tokenId, CancellationToken cancellationToken, bool track = false);
    Task Update(Token token, CancellationToken cancellationToken);
    Task Update(IEnumerable<Token> tokens, CancellationToken cancellationToken);
    Task DeleteTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken);
    Task DeleteToken(Token token, CancellationToken cancellationToken);
    Task<DbPaginationResult<Token>> FindAllTokens(PaginationFilter paginationFilter, Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false);
}
