using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Domain.Entities;

namespace Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;

public interface ITokensRepository
{
    Task Add(Token token);

    Task<DbPaginationResult<Token>> ListTokensAllocatedOrCreatedByWithContent(IEnumerable<string> ids, IdentityAddress activeIdentity, PaginationFilter paginationFilter,
        CancellationToken cancellationToken, bool track = false);

    Task<IEnumerable<Token>> ListWithoutContent(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false);

    Task<Token?> GetWithoutContent(TokenId tokenId, CancellationToken cancellationToken, bool track = false);
    Task<Token?> GetWithContent(TokenId tokenId, CancellationToken cancellationToken, bool track = false);
    Task Update(Token token, CancellationToken cancellationToken);
    Task Update(IEnumerable<Token> tokens, CancellationToken cancellationToken);
    Task DeleteToken(Token token, CancellationToken cancellationToken);
    Task<DbPaginationResult<Token>> ListWithoutContent(PaginationFilter paginationFilter, Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false);
    Task<int> Delete(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken);
}
