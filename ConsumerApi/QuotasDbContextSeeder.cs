using Backbone.BuildingBlocks.API.Extensions;
using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace Backbone.ConsumerApi;

public class QuotasDbContextSeeder : IDbSeeder<QuotasDbContext>
{
    private readonly DevicesDbContext _devicesDbContext;
    private readonly IMetricsRepository _metricsRepository;
    private readonly ITiersRepository _tiersRepository;

    public QuotasDbContextSeeder(DevicesDbContext devicesDbContext, IMetricsRepository metricsRepository, ITiersRepository tiersRepository)
    {
        _devicesDbContext = devicesDbContext;
        _metricsRepository = metricsRepository;
        _tiersRepository = tiersRepository;
    }

    public async Task SeedAsync(QuotasDbContext context)
    {
        await SeedTier(context);
        await AddTierToIdentities(context);
        await EnsureUpForDeletionTierWithQuotas(context);
    }

    private async Task EnsureUpForDeletionTierWithQuotas(QuotasDbContext context)
    {
        Tier upForDeletionTier;

        try
        {
            upForDeletionTier = await _tiersRepository.Find(TierId.UP_FOR_DELETION_DEFAULT_ID, CancellationToken.None, true);
        }
        catch (NotFoundException)
        {
            upForDeletionTier = new Tier(new TierId(TierId.UP_FOR_DELETION_DEFAULT_ID), Tier.UP_FOR_DELETION_TIER_NAME);
            await _tiersRepository.Add(upForDeletionTier, CancellationToken.None);
        }

        var metrics = await _metricsRepository.FindAll(CancellationToken.None);
        var missingMetrics = metrics.Where(metric => upForDeletionTier.Quotas.All(quota => quota.MetricKey.Value != metric.Key.Value));
        foreach (var metric in missingMetrics)
        {
            upForDeletionTier.CreateQuotaForUpForDeletionTier(metric.Key, 0, QuotaPeriod.Day);
        }
        await _tiersRepository.Update(upForDeletionTier, CancellationToken.None);
    }

    private async Task AddTierToIdentities(QuotasDbContext context)
    {
        if (await context.Identities.AnyAsync())
            return;

        foreach (var sourceIdentity in _devicesDbContext.Identities)
        {
            await context.Identities.AddAsync(new Identity(sourceIdentity.Address, new TierId(sourceIdentity.TierId!)));
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedTier(QuotasDbContext context)
    {
        if (await context.Tiers.AnyAsync())
            return;

        foreach (var sourceTier in _devicesDbContext.Tiers)
        {
            await context.Tiers.AddAsync(new Tier(new TierId(sourceTier.Id), sourceTier.Name));
        }

        await context.SaveChangesAsync();
    }
}
