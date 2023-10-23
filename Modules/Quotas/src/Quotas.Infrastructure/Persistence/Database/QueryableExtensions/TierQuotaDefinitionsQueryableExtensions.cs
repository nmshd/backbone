using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Quotas.Domain.Aggregates.Tiers;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Quotas.Infrastructure.Persistence.Database.QueryableExtensions;
public static class TierQuotaDefinitionsQueryableExtensions
{
    public static async Task<TierQuotaDefinition> FirstWithId(this IQueryable<TierQuotaDefinition> query, string id, CancellationToken cancellationToken)
    {
        var tierQuotaDefinition = await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken) ?? throw new NotFoundException(nameof(TierQuotaDefinition));
        return tierQuotaDefinition;
    }
}
