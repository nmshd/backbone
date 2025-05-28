using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.Tier;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Commands.UpdateIdentity;

public class Handler : IRequestHandler<UpdateIdentityCommand>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;

    public Handler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
    }

    public async Task Handle(UpdateIdentityCommand request, CancellationToken cancellationToken)
    {
        var newTierIdResult = TierId.Create(request.TierId);
        var identity = await _identitiesRepository.Get(request.Address, cancellationToken, track: true) ?? throw new NotFoundException(nameof(Identity));

        var tiers = await _tiersRepository.ListByIds(new List<TierId> { identity.TierId, newTierIdResult.Value }, cancellationToken);
        var newTier = tiers.SingleOrDefault(t => t.Id == newTierIdResult.Value) ?? throw new NotFoundException(nameof(Tier));

        identity.ChangeTier(newTier.Id);
        await _identitiesRepository.Update(identity, cancellationToken);
    }
}
