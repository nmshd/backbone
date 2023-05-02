using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Domain.Aggregates.Entities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.API;

public class QuotasDbContextSeed
{
    private readonly DevicesDbContext _devicesDbContext;

    public QuotasDbContextSeed(DevicesDbContext devicesDbContext)
    {
        _devicesDbContext = devicesDbContext;
    }

    public async Task SeedAsync(QuotasDbContext context)
    {
        await SeedTier(context);
        await AddTierToIdentities(context);
    }

    private async Task AddTierToIdentities(QuotasDbContext context)
    {
        if (await context.Identities.AnyAsync())
            return;

        foreach (var sourceIdentity in _devicesDbContext.Identities)
        {
            await context.Identities.AddAsync(new Identity(sourceIdentity.Address, sourceIdentity.TierId));
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedTier(QuotasDbContext context)
    {
        if (await context.Tiers.AnyAsync())
            return;

        foreach (var sourceTier in _devicesDbContext.Tiers)
        {
            await context.Tiers.AddAsync(new Tier(sourceTier.Id, sourceTier.Name));
        }

        await context.SaveChangesAsync();
    }
}
