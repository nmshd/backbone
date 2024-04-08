using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Challenges.Domain.Entities;
using Backbone.Modules.Challenges.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Challenges.Application.Extensions;

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
