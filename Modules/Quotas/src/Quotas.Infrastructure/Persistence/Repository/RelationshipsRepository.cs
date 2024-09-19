using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;

public class RelationshipsRepository : IRelationshipsRepository
{
    private readonly IQueryable<Relationship> _relationshipsReadonly;

    public RelationshipsRepository(QuotasDbContext dbContext)
    {
        _relationshipsReadonly = dbContext.Relationships.AsNoTracking();
    }

    public async Task<uint> Count(string createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var relationshipsCount = await _relationshipsReadonly
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .Where(r => r.Status == RelationshipStatus.Pending && r.From == createdBy ||
                        r.Status == RelationshipStatus.Active && (r.From == createdBy || r.To == createdBy) ||
                        r.Status == RelationshipStatus.Terminated && (r.From == createdBy || r.To == createdBy) ||
                        r.Status == RelationshipStatus.DeletionProposed && (r.FromHasDecomposed && r.To == createdBy || r.ToHasDecomposed && r.From == createdBy))
            .CountAsync(cancellationToken);

        return (uint)relationshipsCount;
    }
}
