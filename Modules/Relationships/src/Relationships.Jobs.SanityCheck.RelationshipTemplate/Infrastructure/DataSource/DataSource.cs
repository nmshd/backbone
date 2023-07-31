using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Domain.Ids;
using Backbone.Modules.Relationships.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly RelationshipsDbContext _dbContext;
    private readonly BlobOptions _blobOptions;
    private const string BLOB_PREFIX = "RLT";

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

    public async Task<IEnumerable<RelationshipTemplateId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SetReadOnly<Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplate>().Select(u => u.Id).ToListAsync(cancellationToken);
    }
}
