using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Relationships.Infrastructure.Extensions;

public static class RelationshipQueryableExtensions
{
    extension(IQueryable<Relationship> query)
    {
        public IQueryable<Relationship> BetweenParticipants(IdentityAddress participant1, IdentityAddress participant2)
        {
            return query.WithParticipant(participant1).WithParticipant(participant2);
        }

        public IQueryable<Relationship> WithParticipant(IdentityAddress participantAddress)
        {
            return query.Where(Relationship.HasParticipant(participantAddress));
        }

        public IQueryable<Relationship> WithIdIn(IEnumerable<RelationshipId> ids)
        {
            return query.Where(r => ids.Contains(r.Id));
        }

        public async Task<Relationship?> FirstWithIdOrDefault(RelationshipId relationshipId, CancellationToken cancellationToken)
        {
            return await query.FirstOrDefaultAsync(r => r.Id == relationshipId, cancellationToken);
        }

        public async Task<Relationship> FirstWithId(RelationshipId relationshipId, CancellationToken cancellationToken)
        {
            var relationship = await query.FirstWithIdOrDefault(relationshipId, cancellationToken) ?? throw new NotFoundException(nameof(Relationship));
            return relationship;
        }
    }
}
