using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public class TokensRepository : ITokensRepository
{
    private readonly TokensDbContext _dbContext;
    private readonly IQueryable<Token> _readonlyTokensDbSet;
    private readonly DbSet<Token> _tokensDbSet;

    public TokensRepository(TokensDbContext dbContext)
    {
        _dbContext = dbContext;
        _tokensDbSet = dbContext.Tokens;
        _readonlyTokensDbSet = dbContext.Tokens.AsNoTracking();
    }

    public async Task<Token> Find(TokenId id, IdentityAddress? activeIdentity)
    {
        var getMetadata = _readonlyTokensDbSet
            .Where(Token.IsNotExpired)
            .Where(Token.CanBeCollectedBy(activeIdentity))
            .FirstWithId(id);

        var token = await getMetadata ?? throw new NotFoundException(nameof(Token));

        return token;
    }

    public async Task<DbPaginationResult<Token>> FindAllWithIds(IdentityAddress activeIdentity, IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return await Find(activeIdentity, ids, paginationFilter, cancellationToken);
    }

    public async Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return await Find(owner, [], paginationFilter, cancellationToken, true);
    }

    private async Task<DbPaginationResult<Token>> Find(IdentityAddress? identityAddress, IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken, bool identityIsOwner = false)
    {
        if (paginationFilter == null)
            throw new Exception("A pagination filter has to be provided.");

        var query = _readonlyTokensDbSet.Where(Token.IsNotExpired);

        if (identityIsOwner)
            query = query.Where(t => t.CreatedBy == identityAddress);

        var idsArray = ids as TokenId[] ?? ids.ToArray();

        if (idsArray.Any())
            query = query.Where(t => idsArray.Contains(t.Id));

        if (identityAddress != null)
            query = query.Where(Token.CanBeCollectedBy(identityAddress));

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return dbPaginationResult;
    }

    #region Write

    public async Task Add(Token token)
    {
        await _tokensDbSet.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken)
    {
        await _tokensDbSet.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}

public static class IDbSetExtensions
{
    public static async Task<Token> FirstWithId(this IQueryable<Token> query, TokenId id)
    {
        var entity = await query.FirstOrDefaultAsync(t => t.Id == id);

        return entity ?? throw new NotFoundException(nameof(Token));
    }
}
