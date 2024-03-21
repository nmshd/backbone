using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Extensions;

public static class RelationshipTemplateQueryableExtensions
{
    public static async Task<RelationshipTemplate?> FirstWithIdOrDefault(this IQueryable<RelationshipTemplate> query, RelationshipTemplateId templateId, CancellationToken cancellationToken)
    {
        var template = await query.FirstOrDefaultAsync(r => r.Id == templateId, cancellationToken);
        return template;
    }

    public static IQueryable<RelationshipTemplate> NotExpiredFor(this IQueryable<RelationshipTemplate> query, IdentityAddress address)
    {
        return query.Where(
            t => t.ExpiresAt == null ||
                 t.ExpiresAt > SystemTime.UtcNow ||
                 t.Relationships.Any(r => r.From == address || r.To == address)
        );
    }

    public static IQueryable<RelationshipTemplate> WithIdIn(this IQueryable<RelationshipTemplate> query, IEnumerable<RelationshipTemplateId> ids)
    {
        return query.Where(t => ids.Contains(t.Id));
    }

    public static IQueryable<RelationshipTemplate> NotDeleted(this IQueryable<RelationshipTemplate> query)
    {
        return query.Where(t => !t.DeletedAt.HasValue);
    }
}
