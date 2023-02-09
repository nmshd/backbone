using Backbone.Modules.Tokens.Domain.Entities;
using Backbone.Modules.Tokens.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Tokens.Jobs.SanityCheck.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobstorage;
        private readonly ApplicationDbContext _dbcontext;

        public DataSource(IBlobStorage blobStorage, ITokensDbContext dbContext)
        {
            _blobstorage = blobStorage;
            _dbcontext = (ApplicationDbContext)dbContext;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobstorage.FindAllAsync();
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TokenId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _dbcontext.Tokens.Select(u => u.Id).ToListAsync(cancellationToken);
        }
    }
}
