using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Users.Commands.SeedTestUsers;

public class Handler : IRequestHandler<SeedTestUsersCommand>
{
    private readonly ApplicationConfiguration _applicationConfiguration;
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private Tier? _basicTier;
    private CancellationToken _cancellationToken;

    public Handler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IOptions<ApplicationConfiguration> applicationOptions)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _applicationConfiguration = applicationOptions.Value;
    }

    public async Task Handle(SeedTestUsersCommand request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _basicTier = (await _tiersRepository.GetBasicTier(cancellationToken))!;

        await CreateIdentityIfNecessary([1, 1, 1, 1, 1], "USRa", "Aaaaaaaa1!");
        await CreateIdentityIfNecessary([2, 2, 2, 2, 2], "USRb", "Bbbbbbbb1!");
    }

    private async Task CreateIdentityIfNecessary(byte[] publicKey, string username, string password)
    {
        var address = IdentityAddress.Create(publicKey, _applicationConfiguration.DidDomainName);

        var identityExists = await _identitiesRepository.Exists(address, _cancellationToken);

        if (!identityExists)
        {
            var identity = Identity.CreateTestIdentity(address, publicKey, _basicTier!.Id, username);
            await _identitiesRepository.Add(identity, password);
        }
    }
}
