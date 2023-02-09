using Backbone.Modules.Files.Application.Infrastructure.Persistence;
using Backbone.Modules.Files.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Files.Jobs.SanityCheck.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobstorage;
        private readonly IFilesDbContext _dbcontext;

        public DataSource(IBlobStorage blobStorage, IFilesDbContext dbContext)
        {
            _blobstorage = blobStorage;
            _dbcontext = dbContext;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobstorage.FindAllAsync();
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FileId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _dbcontext.SetReadOnly<FileMetadata>().Select(u => u.Id).ToListAsync(cancellationToken);
        }
    }
}
