using System.Linq.Expressions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly IQueryable<Relationship> _readonlyRelationships;

    public RelationshipsRepository(DevicesDbContext dbContext)
    {
        _readonlyRelationships = dbContext.Relationships.AsNoTracking();
    }

    public Task<bool> RelationshipExists(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
        return _readonlyRelationships.AnyAsync(filter, cancellationToken);
    }
}
