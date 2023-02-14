using Backbone.Modules.Tokens.Application.Infrastructure;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;

public class TokenRepository : ITokenRepository
{
    private readonly IBlobStorage _blobStorage;
    private readonly TokenRepositoryOptions _options;
    private readonly IQueryable<Token> _readonlyTokensDbSet;
    private readonly DbSet<Token> _tokensDbSet;

    public TokenRepository(ApplicationDbContext dbContext, IBlobStorage blobStorage, IOptions<TokenRepositoryOptions> options)
    {
        _blobStorage = blobStorage;
        _options = options.Value;
        _tokensDbSet = dbContext.Tokens;
        _readonlyTokensDbSet = dbContext.Tokens.AsNoTracking();
    }

    public async Task<Token> Find(TokenId id)
    {
        var getMetadata = _readonlyTokensDbSet
            .Where(Token.IsNotExpired)
            .FirstWithId(id);

        var getContent = _blobStorage.FindAsync(_options.BlobRootFolder, id);

        await Task.WhenAll(getMetadata, getContent);

        var token = await getMetadata;

        if (token == null)
            throw new NotFoundException(nameof(Token));

        token.Content = await getContent;

        return token;
    }

    public async Task<DbPaginationResult<Token>> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter)
    {
        return await Find(null, ids, paginationFilter);
    }

    public async Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter)
    {
        return await Find(owner, Array.Empty<TokenId>(), paginationFilter);
    }

    public async Task<IdentityAddress> GetOwner(TokenId tokenId)
    {
        var result = await _readonlyTokensDbSet.Select(t => new { t.CreatedBy, t.Id }).FirstOrDefaultAsync(t => t.Id == tokenId);

        if (result == null)
            throw new NotFoundException(nameof(Token));

        return result.CreatedBy;
    }

    private async Task<DbPaginationResult<Token>> Find(IdentityAddress owner, IEnumerable<TokenId> ids, PaginationFilter paginationFilter)
    {
        if (paginationFilter == null)
            throw new Exception("A pagination filter has to be provided.");

        var query = _readonlyTokensDbSet.Where(Token.IsNotExpired);

        var idsArray = ids as TokenId[] ?? ids.ToArray();

        if (idsArray.Any())
            query = query.Where(t => idsArray.Contains(t.Id));

        if (owner != null)
            query = query.Where(t => t.CreatedBy == owner);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter);

        await FillContent(dbPaginationResult.ItemsOnPage);

        return dbPaginationResult;
    }

    private async Task FillContent(IEnumerable<Token> tokens)
    {
        var fillContentTasks = tokens.Select(FillContent);
        await Task.WhenAll(fillContentTasks);
    }

    private async Task FillContent(Token token)
    {
        token.Content = await _blobStorage.FindAsync(_options.BlobRootFolder, token.Id);
    }

    #region Write

    public void Add(Token token)
    {
        _tokensDbSet.Add(token);
        _blobStorage.Add(_options.BlobRootFolder, token.Id, token.Content);
    }

    public void AddRange(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            Add(token);
        }
    }

    public void Remove(TokenId id)
    {
        throw new NotImplementedException();
    }

    public void Remove(Token token)
    {
        _tokensDbSet.Remove(token);
        _blobStorage.Remove(_options.BlobRootFolder, token.Id);
    }

    public void RemoveRange(IEnumerable<Token> tokens)
    {
        foreach (var token in tokens)
        {
            Remove(token);
        }
    }

    public void Update(Token entity)
    {
        _tokensDbSet.Update(entity);
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

public class TokenRepositoryOptions
{
    public string BlobRootFolder { get; set; }
}
