using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Backbone.Messages.Application.Infrastructure.Persistence;
using Backbone.Messages.Domain.Entities;
using Backbone.Messages.Domain.Ids;
using Backbone.Messages.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Backbone.Messages.Jobs.SanityCheck.Infrastructure.DataSource;

public class DataSource : IDataSource
{
    private readonly IBlobStorage _blobStorage;
    private readonly BlobOptions _blobOptions;
    private readonly MessagesDbContext _dbContext;

    public DataSource(IBlobStorage blobStorage, IOptions<BlobOptions> blobOptions, MessagesDbContext dbContext)
    {
        _blobStorage = blobStorage;
        _blobOptions = blobOptions.Value;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
    {
        var blobIds = await _blobStorage.FindAllAsync(_blobOptions.RootFolder);
        return await blobIds.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MessageId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SetReadOnly<Message>().Select(u => u.Id).ToListAsync(cancellationToken);
    }
}
