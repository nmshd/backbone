﻿using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Application.Extensions;

public static class TierQueryableExtensions
{
    public static async Task<Tier> GetBasicTier(this IQueryable<Tier> query, CancellationToken cancellationToken)
    {
        var basicTier = await query.FirstOrDefaultAsync(t => t.Name == TierName.BASIC_DEFAULT_NAME, cancellationToken);

        return basicTier;
    }
}
