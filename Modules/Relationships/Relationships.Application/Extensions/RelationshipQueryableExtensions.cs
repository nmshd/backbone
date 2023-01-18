using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Relationships.Common;
using Relationships.Domain.Entities;
using Relationships.Domain.Ids;

namespace Relationships.Application.Extensions;

public static class RelationshipQueryableExtensions
{
    public static IQueryable<Relationship> NotTerminated(this IQueryable<Relationship> query)
    {
        return query.Where(r => r.Status != RelationshipStatus.Terminated);
    }

    public static IQueryable<Relationship> Pending(this IQueryable<Relationship> query)
    {
        return query.Where(r => r.Status == RelationshipStatus.Pending);
    }

    public static IQueryable<Relationship> To(this IQueryable<Relationship> query, IdentityAddress to)
    {
        return query.Where(r => r.To == to);
    }

    public static IQueryable<Relationship> From(this IQueryable<Relationship> query, IdentityAddress from)
    {
        return query.Where(r => r.From == from);
    }

    public static IQueryable<Relationship> BetweenParticipants(this IQueryable<Relationship> query, IdentityAddress participant1, IdentityAddress participant2)
    {
        return query.WithParticipant(participant1).WithParticipant(participant2);
    }

    public static IQueryable<Relationship> WithParticipant(this IQueryable<Relationship> query, IdentityAddress identityId)
    {
        return query.Where(r => r.To == identityId || r.From == identityId);
    }

    public static IQueryable<Relationship> WithIdIn(this IQueryable<Relationship> query, IEnumerable<RelationshipId> ids)
    {
        return query.Where(r => ids.Contains(r.Id));
    }

    public static IQueryable<Relationship> WithId(this IQueryable<Relationship> query, RelationshipId relationshipId)
    {
        return query.Where(r => r.Id == relationshipId);
    }

    public static IQueryable<Relationship> CreatedAt(this IQueryable<Relationship> query, OptionalDateRange createdAt)
    {
        var newQuery = query;

        if (createdAt == null)
            return newQuery;

        if (createdAt.From != default)
            newQuery = newQuery.Where(r => r.CreatedAt >= createdAt.From);

        if (createdAt.To != default)
            newQuery = newQuery.Where(r => r.CreatedAt <= createdAt.To);

        return newQuery;
    }

    public static async Task<Relationship> FirstWithIdOrDefault(this IQueryable<Relationship> query, RelationshipId relationshipId, CancellationToken cancellationToken)
    {
        return await query.FirstOrDefaultAsync(r => r.Id == relationshipId, cancellationToken);
    }

    public static async Task<Relationship> FirstWithId(this IQueryable<Relationship> query, RelationshipId relationshipId, CancellationToken cancellationToken)
    {
        var relationship = await query.FirstWithIdOrDefault(relationshipId, cancellationToken);

        if (relationship == null)
            throw new NotFoundException(nameof(Relationship));

        return relationship;
    }
}
