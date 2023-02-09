using Backbone.Modules.Messages.Application.Infrastructure.Persistence;
using Backbone.Modules.Messages.Domain.Entities;
using Backbone.Modules.Messages.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Messages.Jobs.SanityCheck.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobstorage;
        private readonly IMessagesDbContext _dbcontext;

        public DataSource(IBlobStorage blobStorage, IMessagesDbContext dbContext)
        {
            _blobstorage = blobStorage;
            _dbcontext = dbContext;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobstorage.FindAllAsync();
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MessageId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _dbcontext.SetReadOnly<Message>().Select(u => u.Id).ToListAsync(cancellationToken);
        }
    }
}
