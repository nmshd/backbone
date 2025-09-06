using System.Linq.Expressions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using EFCore.BulkExtensions;
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

    public async Task<DbPaginationResult<Token>> ListTokensAllocatedOrCreatedByWithContent(IEnumerable<string> ids, IdentityAddress activeIdentity,
        PaginationFilter paginationFilter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _tokensDbSet : _readonlyTokensDbSet)
            .IncludeAll(_dbContext)
            .Where(Token.HasAllocationFor(activeIdentity).Or(Token.WasCreatedBy(activeIdentity)))
            .Where(t => ids.Contains(t.Id));

        var templates = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return templates;
    }

    public async Task<IEnumerable<Token>> ListWithoutContent(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        return await (track ? _tokensDbSet : _readonlyTokensDbSet)
            .Where(filter)
            .Include(t => t.Allocations)
            .ToListAsync(cancellationToken);
    }

    public async Task<DbPaginationResult<Token>> ListWithoutContent(PaginationFilter paginationFilter, Expression<Func<Token, bool>> filter, CancellationToken cancellationToken, bool track = false)
    {
        var query = (track ? _tokensDbSet : _readonlyTokensDbSet)
            .Where(filter);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

        return dbPaginationResult;
    }

    public async Task<int> Delete(Expression<Func<Token, bool>> filter, CancellationToken cancellationToken)
    {
#pragma warning disable CS0618 // Type or member is obsolete; While it's true that there is an ExecuteDeleteAsync method in EF Core, it cannot be used here because it cannot be used in scenarios where table splitting is used. See https://github.com/dotnet/efcore/issues/28521 for the feature request that would allow this.
        return await _tokensDbSet.Where(filter).BatchDeleteAsync(cancellationToken);
#pragma warning restore CS0618 // Type or member is obsolete
    }

    public async Task<Token?> GetWithoutContent(TokenId id, CancellationToken cancellationToken, bool track = false)
    {
        var token = await (track ? _tokensDbSet : _readonlyTokensDbSet)
            .Include(t => t.Allocations)
            .FirstOrDefaultAsync(Token.HasId(id), cancellationToken);

        return token;
    }

    public async Task<Token?> GetWithContent(TokenId id, CancellationToken cancellationToken, bool track = false)
    {
        var token = await (track ? _tokensDbSet : _readonlyTokensDbSet)
            .IncludeAll(_dbContext)
            .FirstOrDefaultAsync(Token.HasId(id), cancellationToken);

        return token;
    }

    #region Write

    public async Task Add(Token token)
    {
        await _tokensDbSet.AddAsync(token);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Token token, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(IEnumerable<Token> tokens, CancellationToken cancellationToken)
    {
        _tokensDbSet.UpdateRange(tokens);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteToken(Token token, CancellationToken cancellationToken)
    {
        _tokensDbSet.Remove(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}
