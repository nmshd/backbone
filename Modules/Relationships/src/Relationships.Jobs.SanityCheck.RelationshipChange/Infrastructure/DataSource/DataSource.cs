using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.RelationshipChange.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly RelationshipsDbContext _dbContext;
    private readonly BlobOptions _blobOptions;
    private const string BLOB_PREFIX = "RCH";

    public DataSource(IBlobStorage blobStorage, RelationshipsDbContext dbContext, IOptions<BlobOptions> blobOptions)
    {
        _blobStorage = blobStorage;
        _dbContext = dbContext;
        _blobOptions = blobOptions.Value;
    }

    public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        var blobIds = await _blobStorage.FindAllAsync(_blobOptions.RootFolder, BLOB_PREFIX);
        return await blobIds.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RelationshipChangeId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SetReadOnly<Domain.Entities.RelationshipChange>().Select(u => u.Id).ToListAsync(cancellationToken);
    }
}
