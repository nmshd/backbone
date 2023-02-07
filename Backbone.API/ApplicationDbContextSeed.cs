using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Devices.API;

public class ApplicationDbContextSeed
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

    public async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedEverything(context);
    }

    private async Task SeedEverything(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        await SeedApplicationUsers(context);
    }

    private async Task SeedApplicationUsers(ApplicationDbContext context)
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
                new byte[] { 1, 1, 1, 1, 1 }, 1
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
                new byte[] { 2, 2, 2, 2, 2 }, 1
            )),
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "b");
        await context.Users.AddAsync(user);

        await context.SaveChangesAsync();
    }
}
