using System.Linq.Expressions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
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

    public async Task<bool> RelationshipExists(Expression<Func<Relationship, bool>> filter, CancellationToken cancellationToken)
    {
        return await _readonlyRelationships.AnyAsync(filter, cancellationToken);
    }

    public async Task<List<Relationship>> GetYoungestRelationships(IdentityAddress mainIdentity, IdentityAddress[] peers, CancellationToken cancellationToken)
    {
        var relationships = await _readonlyRelationships
            .Where(Relationship.IsBetween(mainIdentity, peers))
            .GroupBy(r => new { r.From, r.To })
            .Select(g => g.OrderByDescending(gr => gr.CreatedAt).First())
            .ToListAsync(cancellationToken);

        // we need to ensure that we only return distinct relationships, where the order of From and To does not matter
        var distinctRelationships = relationships
            .GroupBy(r => string.Compare(r.From.Value, r.To.Value, StringComparison.Ordinal) > 0 ? new { A = r.From, B = r.To } : new { A = r.To, B = r.From })
            .Select(g => g.OrderByDescending(gr => gr.CreatedAt).First())
            .ToList();

        return distinctRelationships;
    }
}
