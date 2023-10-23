using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;

public static class TiersQueryableExtensions
{
    public static async Task<Tier> FirstWithId(this IQueryable<Tier> query, string id, CancellationToken cancellationToken)
    {
        var tier = await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken) ?? throw new NotFoundException(nameof(Tier));
        return tier;
    }
}
