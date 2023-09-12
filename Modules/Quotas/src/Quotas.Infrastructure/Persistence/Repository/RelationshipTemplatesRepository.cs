using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Relationships;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Quotas.Infrastructure.Persistence.Repository;
internal class RelationshipTemplatesRepository : IRelationshipTemplatesRepository
{
    private readonly IQueryable<RelationshipTemplate> _templatesReadOnly;

    public RelationshipTemplatesRepository(QuotasDbContext dbContext)
    {
        _templatesReadOnly = dbContext.RelationshipTemplates.AsNoTracking();
    }

    public async Task<uint> Count(IdentityAddress createdBy, DateTime createdAtFrom, DateTime createdAtTo, CancellationToken cancellationToken)
    {
        var tokensCount = await _templatesReadOnly.CountAsync(t => t.CreatedBy == createdBy.StringValue, cancellationToken);
        return (uint)tokensCount;
    }
}
