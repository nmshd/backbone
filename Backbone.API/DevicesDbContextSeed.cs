using Backbone.Modules.Devices.Application.Extensions;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backbone.API;

public class DevicesDbContextSeed
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

    public async Task SeedAsync(DevicesDbContext context)
    {
        await SeedEverything(context);
    }

    private async Task SeedEverything(DevicesDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        await SeedApplicationUsers(context);
        await SeedBasicTier(context);
        await AddBasicTierToIdentities(context);
    }

    private async Task SeedApplicationUsers(DevicesDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var user = new ApplicationUser
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRa",
            NormalizedUserName = "USRA",
            Device = new Device(new Identity("test",
                IdentityAddress.Create(new byte[] { 1, 1, 1, 1, 1 }, "id1"),
                new byte[] { 1, 1, 1, 1, 1 }, await context.Tiers.BasicTier(CancellationToken.None), 1
            )),
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "a");
        await context.Users.AddAsync(user);

        user = new ApplicationUser
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRb",
            NormalizedUserName = "USRB",
            Device = new Device(new Identity("test",
                IdentityAddress.Create(new byte[] { 2, 2, 2, 2, 2 }, "id1"),
                new byte[] { 2, 2, 2, 2, 2 }, await context.Tiers.BasicTier(CancellationToken.None), 1
            )),
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "b");
        await context.Users.AddAsync(user);

        await context.SaveChangesAsync();
    }

    private async Task SeedBasicTier(DevicesDbContext context)
    {
        if (await context.Tiers.AnyAsync())
            return;

        await context.Tiers.AddAsync(new Tier(TierName.Create(TierName.BASIC_DEFAULT_NAME).Value));

        await context.SaveChangesAsync();
    }

    private async Task AddBasicTierToIdentities(DevicesDbContext context)
    {
        try
        {
            var basicTier = await context.Tiers.BasicTier(CancellationToken.None);
            var identitiesWithoutTier = from i in context.Identities where i.Tier == null select i;
            await identitiesWithoutTier.ForEachAsync(it =>
            {
                it.Tier = basicTier;
            });

            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new Exception("Basic Tier was not found.");
        }
    }
}
