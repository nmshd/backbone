using Backbone.Modules.Relationships.Application.Infrastructure;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.BlobStorage;
using Microsoft.EntityFrameworkCore;

namespace Relationships.Jobs.SanityCheck.RelationshipTemplate.Infrastructure.DataSource
{
    public class DataSource : IDataSource
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IRelationshipsDbContext _dbContext;
        private const string BLOB_PREFIX = "RLT";

        public DataSource(IBlobStorage blobStorage, IRelationshipsDbContext dbContext)
        {
            _blobStorage = blobStorage;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> GetBlobIdsAsync(CancellationToken cancellationToken)
        {
            var blobIds = await _blobStorage.FindAllAsync(BLOB_PREFIX);
            return await blobIds.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RelationshipTemplateId>> GetDatabaseIdsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SetReadOnly<Backbone.Modules.Relationships.Domain.Entities.RelationshipTemplate>().Select(u => u.Id).ToListAsync(cancellationToken);
        }
    }
}