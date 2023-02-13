using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Files.Jobs.SanityCheck.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IFilesDbContext _dbContext;

        public DataSource(IBlobStorage blobStorage, IFilesDbContext dbContext)
        {
            _blobStorage = blobStorage;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobStorage.FindAllAsync();
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SetReadOnly<FileMetadata>().Select(u => u.Id).ToListAsync(cancellationToken);
        }
    }
}
