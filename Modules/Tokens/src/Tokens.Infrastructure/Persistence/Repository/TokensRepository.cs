using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Application.Tokens.Queries.ListTokens;
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

    public async Task<DbPaginationResult<Token>> FindTokens(IEnumerable<ListTokensQueryItem> queryItems, IdentityAddress activeIdentity,
        PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var queryItemsList = queryItems.ToList();

        Expression<Func<Token, bool>> idAndPasswordFilter = template => false;

        foreach (var inputQuery in queryItemsList)
        {
            idAndPasswordFilter = idAndPasswordFilter
                .Or(Token.HasId(TokenId.Parse(inputQuery.Id))
                    .And(Token.CanBeCollectedWithPassword(activeIdentity, inputQuery.Password)));
        }

        var query = (track ? _tokensDbSet : _readonlyTokensDbSet)
            .Where(Token.IsNotExpired)
            .Where(Token.CanBeCollectedBy(activeIdentity))
            .Where(idAndPasswordFilter);

        var templates = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return templates;
    }

    public async Task<IEnumerable<Token>> FindTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _tokensDbSet : _readonlyTokensDbSet)
            .Where(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<DbPaginationResult<Token>> FindAllTokens(PaginationFilter paginationFilter, Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _tokensDbSet : _readonlyTokensDbSet)
            .Where(filter);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return dbPaginationResult;
    }

    public async Task<Token?> Find(TokenId id, IdentityAddress? activeIdentity, CancellationToken cancellationToken, bool track = false)
    {
        var token = await _readonlyTokensDbSet
            .Where(Token.IsNotExpired)
            .Where(Token.CanBeCollectedBy(activeIdentity))
            .Where(Token.HasId(id))
            .FirstOrDefaultAsync(cancellationToken);

        return token;
    }

    public async Task<DbPaginationResult<Token>> FindAllWithIds(IdentityAddress activeIdentity, IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        if (paginationFilter == null)
            throw new Exception("A pagination filter has to be provided.");

        var query = _readonlyTokensDbSet.Where(Token.IsNotExpired);

        var idsArray = ids as TokenId[] ?? ids.ToArray();

        if (idsArray.Length != 0)
            query = query.Where(t => idsArray.Contains(t.Id));

        query = query.Where(Token.CanBeCollectedBy(activeIdentity));

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return dbPaginationResult;
    }

    #region Write

    public async Task Add(Token token)
    {
        await _tokensDbSet.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(IEnumerable<Token> tokens, CancellationToken cancellationToken)
    {
        _tokensDbSet.UpdateRange(tokens);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTokens(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken)
    {
        await _tokensDbSet.Where(filter).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteToken(Token token, CancellationToken cancellationToken)
    {
        _tokensDbSet.Remove(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
