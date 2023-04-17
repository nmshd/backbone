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

        await SeedBasicTier(context);
        await SeedApplicationUsers(context);
        await AddBasicTierToIdentities(context);
    }
    static private async Task<Tier> GetBasicTier(DevicesDbContext context)
    {
        return await context.Tiers.GetBasicTier(CancellationToken.None) ?? throw new Exception("Basic Tier was not found.");
    }

    private async Task SeedApplicationUsers(DevicesDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var basicTier = await GetBasicTier(context);

        var user = new ApplicationUser
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRa",
            NormalizedUserName = "USRA",
            Device = new Device(new Identity("test",
                IdentityAddress.Create(new byte[] { 1, 1, 1, 1, 1 }, "id1"),
                new byte[] { 1, 1, 1, 1, 1 }, basicTier.Id, 1
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
                new byte[] { 2, 2, 2, 2, 2 }, basicTier.Id, 1
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
        var basicTier = await GetBasicTier(context);
        await context.Identities.Where(i => i.TierId == null).ExecuteUpdateAsync(s => s.SetProperty(i => i.TierId, basicTier.Id));
    }
}
