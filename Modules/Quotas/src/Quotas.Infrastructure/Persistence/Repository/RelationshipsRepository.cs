using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
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
        var relationshipsCount = 0; // _relationshipsReadonly.Count();
        return (uint)relationshipsCount;
    }
}
