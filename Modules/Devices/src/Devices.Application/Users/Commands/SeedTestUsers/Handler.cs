using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Devices.Application.Users.Commands.SeedTestUsers;
public class Handler : IRequestHandler<SeedTestUsersCommand, SeedTestUsersResponse>
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();
    private readonly DbSet<ApplicationUser> _usersDb;
    private readonly IDevicesDbContext _context;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IDevicesDbContext context, ITiersRepository tiersRepository)
    {
        _usersDb = context.Set<ApplicationUser>();
        _context = context;
        _tiersRepository = tiersRepository;
    }

    public async Task<SeedTestUsersResponse> Handle(SeedTestUsersCommand request, CancellationToken cancellationToken)
    {
        var basicTier = await _tiersRepository.GetBasicTierAsync(cancellationToken);

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
        await _usersDb.AddAsync(user);

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
        await _usersDb.AddAsync(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new SeedTestUsersResponse();
    }
}
