using Backbone.Modules.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Tokens.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.Infrastructure.Persistence.Database.Repository;

public class TokensRepository : ITokensRepository
{
    private readonly IBlobStorage _blobStorage;
    private readonly tokensRepositoryOptions _options;
    private readonly IQueryable<Token> _readonlyTokensDbSet;
    private readonly DbSet<Token> _tokensDbSet;

    public TokensRepository(TokensDbContext dbContext, IBlobStorage blobStorage, IOptions<tokensRepositoryOptions> options)
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

    public async Task<IEnumerable<TokenId>> GetAllTokenIds(bool includeExpired = false)
    {
        var query = _readonlyTokensDbSet;

        if (!includeExpired)
            query = query.Where(Token.IsNotExpired);

        return await _readonlyTokensDbSet.Select(t => t.Id).ToListAsync();
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

    public async Task Add(Token token)
    {
        await _tokensDbSet.AddAsync(token);
        _blobStorage.Add(_options.BlobRootFolder, token.Id, token.Content);
    }

    public async Task Remove(Token token)
    {
        await _tokensDbSet.AddAsync(token);
        _blobStorage.Remove(_options.BlobRootFolder, token.Id);
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

public class tokensRepositoryOptions
{
    public string BlobRootFolder { get; set; }
}
