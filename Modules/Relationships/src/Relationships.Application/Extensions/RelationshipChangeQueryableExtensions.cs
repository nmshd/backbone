using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Relationships.Common;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;

namespace Relationships.Application.Extensions;

public static class RelationshipChangeQueryableExtensions
{
    public static IQueryable<RelationshipChange> CompletedAt(this IQueryable<RelationshipChange> query, OptionalDateRange completedAt)
    {
        var newQuery = query;

        if (completedAt == null)
            return newQuery;

        if (completedAt.From != default)
            newQuery = newQuery.Where(r => r.Response.CreatedAt >= completedAt.From);

        if (completedAt.To != default)
            newQuery = newQuery.Where(r => r.Response.CreatedAt <= completedAt.To);

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

        if (createdAt == null)
            return newQuery;

        if (createdAt.From != default)
            newQuery = newQuery.Where(r => r.Request.CreatedAt >= createdAt.From);

        if (createdAt.To != default)
            newQuery = newQuery.Where(r => r.Request.CreatedAt <= createdAt.To);

        return newQuery;
    }

    public static IQueryable<RelationshipChange> ModifiedAt(this IQueryable<RelationshipChange> query, OptionalDateRange modifiedAt)
    {
        var newQuery = query;

        if (modifiedAt == null)
            return newQuery;

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

    public static IQueryable<RelationshipChange> WithType(this IQueryable<RelationshipChange> query, RelationshipChangeType? type)
    {
        return type != null ? query.Where(r => r.Type == type) : query;
    }

    public static IQueryable<RelationshipChange> WithStatus(this IQueryable<RelationshipChange> query, RelationshipChangeStatus? status)
    {
        return status != null ? query.Where(r => r.Status == status) : query;
    }

    public static IQueryable<RelationshipChange> WithRelationshipParticipant(this IQueryable<RelationshipChange> query, IdentityAddress identityId)
    {
        return query.Where(r => r.Relationship.From == identityId || r.Relationship.To == identityId);
    }

    public static IQueryable<RelationshipChange> CreatedBy(this IQueryable<RelationshipChange> query, IdentityAddress identityId)
    {
        return identityId != null ? query.Where(r => r.Request.CreatedBy == identityId) : query;
    }

    public static IQueryable<RelationshipChange> CompletedBy(this IQueryable<RelationshipChange> query, IdentityAddress identityId)
    {
        return identityId != null ? query.Where(r => r.Response != null && r.Response.CreatedBy == identityId) : query;
    }

    public static IQueryable<RelationshipChange> WithId(this IQueryable<RelationshipChange> query, RelationshipChangeId id)
    {
        return id != null ? query.Where(r => r.Id == id) : query;
    }
}
