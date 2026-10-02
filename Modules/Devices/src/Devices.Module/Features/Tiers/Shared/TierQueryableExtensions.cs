using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Module.Features.Tiers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Module.Features.Tiers.Shared;

public static class TierQueryableExtensions
{
    public static async Task<Tier?> GetBasicTier(this IQueryable<Tier> query, CancellationToken cancellationToken)
    {
        var basicTier = await query.FirstOrDefaultAsync(t => t.Name == TierName.BASIC_DEFAULT_NAME, cancellationToken);
        return basicTier;
    }
}
