using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Quotas.Domain.Aggregates.Relationships;
using Backbone.Quotas.Infrastructure.Persistence.Database;
using Backbone.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Quotas.Infrastructure.Persistence.Repository;
public class RelationshipTemplatesRepository : IRelationshipTemplatesRepository
{
    private readonly IQueryable<RelationshipTemplate> _readOnlyTemplates;

    public RelationshipTemplatesRepository(QuotasDbContext dbContext)
    {
        _readOnlyTemplates = dbContext.RelationshipTemplates.AsNoTracking();
    }

    public async Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var relationshipTemplatesCount = await _readOnlyTemplates
            .CreatedInInterval(createdAtFrom, createdAtTo)
            .CountAsync(t => t.CreatedBy == createdBy.StringValue, cancellationToken);
        return (uint)relationshipTemplatesCount;
    }
}
