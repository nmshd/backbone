using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Application.Extensions;

public static class RelationshipTemplateQueryableExtensions
{
    public static async Task<RelationshipTemplate> FirstWithId(this IQueryable<RelationshipTemplate> query, RelationshipTemplateId templateId, CancellationToken cancellationToken)
    {
        var template = await query.FirstOrDefaultAsync(r => r.Id == templateId, cancellationToken);

        if (template == null)
            throw new NotFoundException(nameof(RelationshipTemplate));

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
