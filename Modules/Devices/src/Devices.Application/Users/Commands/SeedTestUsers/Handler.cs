using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Tooling;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Users.Commands.SeedTestUsers;

public class Handler : IRequestHandler<SeedTestUsersCommand>
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ApplicationOptions _applicationOptions;
    private readonly IDevicesDbContext _dbContext;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IDevicesDbContext context, ITiersRepository tiersRepository, IPasswordHasher<ApplicationUser> passwordHasher, IOptions<ApplicationOptions> applicationOptions)
    {
        _dbContext = context;
        _tiersRepository = tiersRepository;
        _passwordHasher = passwordHasher;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(SeedTestUsersCommand request, CancellationToken cancellationToken)
    {
        var basicTier = await _tiersRepository.FindBasicTier(cancellationToken);

        var user = new ApplicationUser(new Device(new Identity("test",
            IdentityAddress.Create([1, 1, 1, 1, 1], _applicationOptions.InstanceUrl),
            [1, 1, 1, 1, 1], basicTier!.Id, 1
        ), CommunicationLanguage.DEFAULT_LANGUAGE))
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRa",
            NormalizedUserName = "USRA",
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "a");
        await _dbContext.Set<ApplicationUser>().AddAsync(user, cancellationToken);

        user = new ApplicationUser(new Device(new Identity("test",
            IdentityAddress.Create([2, 2, 2, 2, 2], _applicationOptions.InstanceUrl),
            [2, 2, 2, 2, 2], basicTier.Id, 1
        ), CommunicationLanguage.DEFAULT_LANGUAGE))
        {
            SecurityStamp = Guid.NewGuid().ToString("D"),
            UserName = "USRb",
            NormalizedUserName = "USRB",
            CreatedAt = SystemTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "b");
        await _dbContext.Set<ApplicationUser>().AddAsync(user, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
