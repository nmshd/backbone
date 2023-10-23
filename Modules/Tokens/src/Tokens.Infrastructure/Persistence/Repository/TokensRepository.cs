using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Backbone.BuildingBlocks.Application.Extensions;
using Backbone.BuildingBlocks.Application.Pagination;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tokens.Application.Infrastructure.Persistence.Repository;
using Backbone.Tokens.Domain.Entities;
using Backbone.Tokens.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Tokens.Infrastructure.Persistence.Repository;

public class TokensRepository : ITokensRepository
{
    private readonly IBlobStorage _blobStorage;
    private readonly TokensRepositoryOptions _options;
    private readonly TokensDbContext _dbContext;
    private readonly IQueryable<Token> _readonlyTokensDbSet;
    private readonly DbSet<Token> _tokensDbSet;

    public TokensRepository(TokensDbContext dbContext, IBlobStorage blobStorage, IOptions<TokensRepositoryOptions> options)
    {
        _blobStorage = blobStorage;
        _options = options.Value;
        _dbContext = dbContext;
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

        var token = await getMetadata ?? throw new NotFoundException(nameof(Token));
        token.Content = await getContent;

        return token;
    }

    public async Task<DbPaginationResult<Token>> FindAllWithIds(IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return await Find(null, ids, paginationFilter, cancellationToken);
    }

    public async Task<DbPaginationResult<Token>> FindAllOfOwner(IdentityAddress owner, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        return await Find(owner, Array.Empty<TokenId>(), paginationFilter, cancellationToken);
    }

    public async Task<IEnumerable<TokenId>> GetAllTokenIds(bool includeExpired = false)
    {
        var query = _readonlyTokensDbSet;

        if (!includeExpired)
            query = query.Where(Token.IsNotExpired);

        return await _readonlyTokensDbSet.Select(t => t.Id).ToListAsync();
    }

    private async Task<DbPaginationResult<Token>> Find(IdentityAddress owner, IEnumerable<TokenId> ids, PaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        if (paginationFilter == null)
            throw new Exception("A pagination filter has to be provided.");

        var query = _readonlyTokensDbSet.Where(Token.IsNotExpired);

        var idsArray = ids as TokenId[] ?? ids.ToArray();

        if (idsArray.Any())
            query = query.Where(t => idsArray.Contains(t.Id));

        if (owner != null)
            query = query.Where(t => t.CreatedBy == owner);

        var dbPaginationResult = await query.OrderAndPaginate(d => d.CreatedAt, paginationFilter, cancellationToken);

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
        await _blobStorage.SaveAsync();
        await _dbContext.SaveChangesAsync();
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

public class TokensRepositoryOptions
{
    public string BlobRootFolder { get; set; }
}
