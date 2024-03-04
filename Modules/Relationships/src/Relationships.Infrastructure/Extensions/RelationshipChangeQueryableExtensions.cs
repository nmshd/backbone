using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Common;
using Backbone.Modules.Relationships.Domain.Entities;
using Backbone.Modules.Relationships.Domain.Ids;

namespace Backbone.Modules.Relationships.Infrastructure.Extensions;

public static class RelationshipChangeQueryableExtensions
{
    public static IQueryable<RelationshipChange> CompletedAt(this IQueryable<RelationshipChange> query, OptionalDateRange? completedAt)
    {
        var newQuery = query;

        if (completedAt == null)
            return newQuery;

        if (completedAt.From.HasValue)
            newQuery = newQuery.Where(r => r.Response != null && r.Response.CreatedAt >= completedAt.From);

        if (completedAt.To.HasValue)
            newQuery = newQuery.Where(r => r.Response != null && r.Response.CreatedAt <= completedAt.To);

        return newQuery;
    }

    public static IQueryable<RelationshipChange> OnlyPeerChanges(this IQueryable<RelationshipChange> query, IdentityAddress activeIdentity)
    {
        return query.Where(c => c.Request.CreatedBy != activeIdentity && c.Response == null || c.Response != null && c.Response.CreatedBy != activeIdentity);
    }

    public static IQueryable<RelationshipChange> WithIdIn(this IQueryable<RelationshipChange> query, IEnumerable<RelationshipChangeId> ids)
    {
        return query.Where(c => ids.Contains(c.Id));
    }

    public static IQueryable<RelationshipChange> CreatedAt(this IQueryable<RelationshipChange> query, OptionalDateRange createdAt)
    {
        var newQuery = query;

        if (createdAt.From != default)
            newQuery = newQuery.Where(r => r.Request.CreatedAt >= createdAt.From);

        if (createdAt.To != default)
            newQuery = newQuery.Where(r => r.Request.CreatedAt <= createdAt.To);

        return newQuery;
    }

    public static IQueryable<RelationshipChange> ModifiedAt(this IQueryable<RelationshipChange> query, OptionalDateRange modifiedAt)
    {
        var newQuery = query;

        if (modifiedAt.From != default)
            newQuery = newQuery.Where(c => c.Request.CreatedAt >= modifiedAt.From ||
                                           c.Response != null && c.Response.CreatedAt >= modifiedAt.From);

        if (modifiedAt.To != default)
            newQuery = newQuery.Where(c => c.Request.CreatedAt <= modifiedAt.To ||
                                           c.Response != null && c.Response.CreatedAt <= modifiedAt.To);

        return newQuery;
    }

    public static IQueryable<RelationshipChange> Pending(this IQueryable<RelationshipChange> query)
    {
        return query.Where(r => r.Status == RelationshipChangeStatus.Pending);
    }

    public static IQueryable<RelationshipChange> WithType(this IQueryable<RelationshipChange> query, RelationshipChangeType type)
    {
        return query.Where(r => r.Type == type);
    }

    public static IQueryable<RelationshipChange> WithStatus(this IQueryable<RelationshipChange> query, RelationshipChangeStatus status)
    {
        return query.Where(r => r.Status == status);
    }

    public static IQueryable<RelationshipChange> WithRelationshipParticipant(this IQueryable<RelationshipChange> query, IdentityAddress identityAddress)
    {
        return query.Where(r => r.Relationship.From == identityAddress || r.Relationship.To == identityAddress);
    }

    public static IQueryable<RelationshipChange> CreatedBy(this IQueryable<RelationshipChange> query, IdentityAddress identityId)
    {
        return query.Where(r => r.Request.CreatedBy == identityId);
    }

    public static IQueryable<RelationshipChange> CompletedBy(this IQueryable<RelationshipChange> query, IdentityAddress identityId)
    {
        return query.Where(r => r.Response != null && r.Response.CreatedBy == identityId);
    }

    public static IQueryable<RelationshipChange> WithId(this IQueryable<RelationshipChange> query, RelationshipChangeId id)
    {
        return query.Where(r => r.Id == id);
    }
}
