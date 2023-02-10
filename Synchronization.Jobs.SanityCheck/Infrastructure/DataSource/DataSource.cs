using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Synchronization.Jobs.SanityCheck.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobstorage;
        private readonly ISynchronizationDbContext _dbcontext;

        public DataSource(IBlobStorage blobStorage, ISynchronizationDbContext dbContext)
        {
            _blobstorage = blobStorage;
            _dbcontext = dbContext;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobstorage.FindAllAsync();
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DatawalletModificationId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _dbcontext.SetReadOnly<DatawalletModification>().Select(u => u.Id).ToListAsync(cancellationToken);
        }
    }
}