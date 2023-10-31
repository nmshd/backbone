using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Tokens.Jobs.SanityCheck.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly TokensRepositoryOptions _repositoryOptions;
    private readonly TokensDbContext _dbContext;

    public DataSource(IBlobStorage blobStorage, IOptions<TokensRepositoryOptions> repositoryOptions, TokensDbContext dbContext)
    {
        _blobStorage = blobStorage;
        _repositoryOptions = repositoryOptions.Value;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        var blobIds = await _blobStorage.FindAllAsync(_repositoryOptions.BlobRootFolder);
        return await blobIds.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TokenId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Token>().AsNoTracking().Select(t => t.Id).ToListAsync(cancellationToken);
    }
}
