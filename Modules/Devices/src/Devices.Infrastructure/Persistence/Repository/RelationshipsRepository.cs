using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Relationships;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Infrastructure.Persistence.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly IQueryable<Relationship> _readRelationships;

    public RelationshipsRepository(DevicesDbContext dbContext)
    {
        _readRelationships = dbContext.Relationships.AsNoTracking();
    }

    public async Task<bool> RelationshipExistsBetween(IdentityAddress identityAddress1, IdentityAddress identityAddress2)
    {
        var identitiesSet = new HashSet<string> { identityAddress1, identityAddress2 };

        return await _readRelationships
            .AnyAsync(r => identitiesSet.Contains(r.From) && identitiesSet.Contains(r.To));
    }
}
