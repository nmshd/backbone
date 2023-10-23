using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Challenges.Domain.Entities;
using Backbone.Challenges.Domain.Ids;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Challenges.Application.Extensions;

public static class ChallengeQueryableExtensions
{
    public static IQueryable<Challenge> CreatedBy(this IQueryable<Challenge> query, IdentityAddress createdBy)
    {
        return query.Where(e => e.CreatedBy == createdBy);
    }

    public static IQueryable<Challenge> NotExpired(this IQueryable<Challenge> query)
    {
        return query.Where(Challenge.IsNotExpired);
    }

    public static async Task<Challenge> FirstWithId(this IQueryable<Challenge> query, ChallengeId id, CancellationToken cancellationToken)
    {
        var challenge = await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken) ?? throw new NotFoundException(nameof(Challenge));
        return challenge;
    }
}
