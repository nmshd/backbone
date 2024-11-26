using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Backbone.Modules.Devices.Application.Users.Commands.SeedTestUsers;

public class Handler : IRequestHandler<SeedTestUsersCommand>
{
    private readonly ApplicationOptions _applicationOptions;
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IOptions<ApplicationOptions> applicationOptions)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task Handle(SeedTestUsersCommand request, CancellationToken cancellationToken)
    {
        var basicTier = await _tiersRepository.FindBasicTier(cancellationToken);

        await CreateIdentityAIfNecessary(basicTier!, cancellationToken);
        await CreateIdentityBIfNecessary(basicTier!, cancellationToken);
    }

    private async Task CreateIdentityAIfNecessary(Tier basicTier, CancellationToken cancellationToken)
    {
        var addressOfIdentityA = IdentityAddress.Create([1, 1, 1, 1, 1], _applicationOptions.DidDomainName);
        var identityA = await _identitiesRepository.FindByAddress(addressOfIdentityA, cancellationToken);
        if (identityA == null)
        {
            identityA = Identity.CreateTestIdentity(addressOfIdentityA, [1, 1, 1, 1, 1], basicTier.Id, "USRa");
            await _identitiesRepository.Add(identityA, "Aaaaaaaa1!");
        }
    }

    private async Task CreateIdentityBIfNecessary(Tier basicTier, CancellationToken cancellationToken)
    {
        var addressOfIdentityB = IdentityAddress.Create([2, 2, 2, 2, 2], _applicationOptions.DidDomainName);
        var identityB = await _identitiesRepository.FindByAddress(addressOfIdentityB, cancellationToken);
        if (identityB == null)
        {
            identityB = Identity.CreateTestIdentity(addressOfIdentityB, [2, 2, 2, 2, 2], basicTier.Id, "USRb");
            await _identitiesRepository.Add(identityB, "Bbbbbbbb1!");
        }
    }
}
