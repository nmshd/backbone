﻿using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Application.Tiers.Commands.CreateTier;
using Backbone.Modules.Devices.Application.Users.Commands.SeedTestUsers;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backbone.API;

public class DevicesDbContextSeed
{
    private readonly IMediator _mediator;

    public DevicesDbContextSeed(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task SeedAsync(DevicesDbContext context)
    {
        await SeedEverything(context);
    }

    private async Task SeedEverything(DevicesDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        await SeedBasicTier(context);
        await SeedApplicationUsers(context);
        await AddBasicTierToIdentities(context);
    }
    private static async Task<Tier> GetBasicTier(DevicesDbContext context)
    {
        return await context.Tiers.GetBasicTier(CancellationToken.None) ?? throw new Exception("Basic Tier was not found.");
    }

    private async Task SeedApplicationUsers(DevicesDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        await _mediator.Send(new SeedTestUsersCommand());
    }

    private async Task SeedBasicTier(DevicesDbContext context)
    {
        try
        {
            var basicTier = await GetBasicTier(context);
            return;
        }
        catch (Exception)
        {
            await _mediator.Send(new CreateTierCommand(TierName.BASIC_DEFAULT_NAME));
        }
    }

    private async Task AddBasicTierToIdentities(DevicesDbContext context)
    {
        var basicTier = await GetBasicTier(context);
        await context.Identities.Where(i => i.TierId == null).ExecuteUpdateAsync(s => s.SetProperty(i => i.TierId, basicTier.Id));
    }
}
