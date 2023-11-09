﻿using Backbone.BuildingBlocks.API.Extensions;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Quotas.Application.Tiers.Commands.SeedUpForDeletionTier;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Infrastructure.Persistence.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Backbone.ConsumerApi;

public class QuotasDbContextSeeder : IDbSeeder<QuotasDbContext>
{
    private readonly DevicesDbContext _devicesDbContext;
    private readonly IMediator _mediator;

    public QuotasDbContextSeeder(DevicesDbContext devicesDbContext, IMediator mediator)
    {
        _devicesDbContext = devicesDbContext;
        _mediator = mediator;
    }

    public async Task SeedAsync(QuotasDbContext context)
    {
        await SeedTier(context);
        await AddTierToIdentities(context);
        await EnsureUpForDeletionTierWithQuotas(context);
    }

    private async Task EnsureUpForDeletionTierWithQuotas(QuotasDbContext context)
    {
        await _mediator.Send(new SeedUpForDeletionTierCommand());
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
