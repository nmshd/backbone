using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.RelationshipTemplates;
using Backbone.Tooling;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Extensions;

public static class RelationshipTemplateQueryableExtensions
{
    extension(IQueryable<RelationshipTemplate> query)
    {
        public async Task<RelationshipTemplate?> FirstWithIdOrDefault(RelationshipTemplateId templateId, CancellationToken cancellationToken)
        {
            var template = await query.FirstOrDefaultAsync(r => r.Id == templateId, cancellationToken);
            return template;
        }

        public IQueryable<RelationshipTemplate> NotExpiredFor(IdentityAddress address)
        {
            return query.Where(t => t.ExpiresAt == null ||
                                    t.ExpiresAt > SystemTime.UtcNow ||
                                    t.Relationships.Any(r => r.From == address || r.To == address)
            );
        }

        public IQueryable<RelationshipTemplate> WithIdIn(IEnumerable<RelationshipTemplateId> ids)
        {
            return query.Where(t => ids.Contains(t.Id));
        }
    }
}
