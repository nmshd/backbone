using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Extensions;

public static class RelationshipQueryableExtensions
{
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

    public static async Task<Relationship?> FirstWithIdOrDefault(this IQueryable<Relationship> query, RelationshipId relationshipId, CancellationToken cancellationToken)
    {
        return await query.FirstOrDefaultAsync(r => r.Id == relationshipId, cancellationToken);
    }

    public static async Task<Relationship> FirstWithId(this IQueryable<Relationship> query, RelationshipId relationshipId, CancellationToken cancellationToken)
    {
        var relationship = await query.FirstWithIdOrDefault(relationshipId, cancellationToken) ?? throw new NotFoundException(nameof(Relationship));
        return relationship;
    }
}
