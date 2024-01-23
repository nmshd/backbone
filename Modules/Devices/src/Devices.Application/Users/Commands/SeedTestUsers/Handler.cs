using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities;
using Backbone.Tooling;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Backbone.Modules.Devices.Application.Users.Commands.SeedTestUsers;
public class Handler : IRequestHandler<SeedTestUsersCommand>
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IDevicesDbContext _dbContext;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IDevicesDbContext context, ITiersRepository tiersRepository, IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _dbContext = context;
        _tiersRepository = tiersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(SeedTestUsersCommand request, CancellationToken cancellationToken)
    {
        var basicTier = await _tiersRepository.GetBasicTierAsync(cancellationToken);

        var user = new ApplicationUser
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRa",
            NormalizedUserName = "USRA",
            Device = new Device(new Identity("test",
                IdentityAddress.Create([1, 1, 1, 1, 1], "id1"),
                [1, 1, 1, 1, 1], basicTier!.Id, 1
            )),
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "a");
        await _dbContext.Set<ApplicationUser>().AddAsync(user, cancellationToken);

        user = new ApplicationUser
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRb",
            NormalizedUserName = "USRB",
            Device = new Device(new Identity("test",
                IdentityAddress.Create([2, 2, 2, 2, 2], "id1"),
                [2, 2, 2, 2, 2], basicTier.Id, 1
            )),
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "b");
        await _dbContext.Set<ApplicationUser>().AddAsync(user, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
