using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
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

        var identityA = Identity.CreateTestIdentity(IdentityAddress.Create([1, 1, 1, 1, 1], _applicationOptions.DidDomainName), [1, 1, 1, 1, 1], basicTier!.Id, "USRa");
        var identityB = Identity.CreateTestIdentity(IdentityAddress.Create([2, 2, 2, 2, 2], _applicationOptions.DidDomainName), [2, 2, 2, 2, 2], basicTier.Id, "USRb");

        await _identitiesRepository.Add(identityA, "Aaaaaaaa1!");
        await _identitiesRepository.Add(identityB, "Bbbbbbbb1!");
    }
}
