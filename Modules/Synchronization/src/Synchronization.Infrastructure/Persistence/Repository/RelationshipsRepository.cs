using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.Entities.Relationships;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Infrastructure.Persistence.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly IQueryable<Relationship> _relationshipsReadOnly;

    public RelationshipsRepository(SynchronizationDbContext dbContext)
    {
        _relationshipsReadOnly = dbContext.Relationships.AsNoTracking();
    }

    public async Task<List<Relationship>> ListRelationships(IEnumerable<RelationshipId> ids, CancellationToken cancellationToken)
    {
        return await _relationshipsReadOnly.Where(r => ids.Contains(r.Id)).ToListAsync(cancellationToken);
    }
}
