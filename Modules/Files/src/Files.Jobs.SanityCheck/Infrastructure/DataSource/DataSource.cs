using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using File = Backbone.Modules.Files.Domain.Entities.File;

namespace Files.Jobs.SanityCheck.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly IFilesDbContext _dbContext;
    private readonly BlobOptions _blobOptions;

    public DataSource(IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, IFilesDbContext dbContext)
    {
        _blobStorage = blobStorage;
        _dbContext = dbContext;
        _blobOptions = blobOptions.Value;
    }

    public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        var blobIds = await _blobStorage.FindAllAsync(_blobOptions.RootFolder);
        return await blobIds.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SetReadOnly<File>().Select(u => u.Id).ToListAsync(cancellationToken);
    }
}
