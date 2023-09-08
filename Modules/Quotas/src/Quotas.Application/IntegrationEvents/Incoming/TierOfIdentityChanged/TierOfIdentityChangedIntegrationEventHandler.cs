using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEventHandler : IIntegrationEventHandler<TierOfIdentityChangedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    public TierOfIdentityChangedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
    }

    public async Task Handle(TierOfIdentityChangedIntegrationEvent @event)
    {
        var identity = await _identitiesRepository.Find(@event.IdentityAddress, CancellationToken.None, track: true);
        var newTier = await _tiersRepository.Find(@event.NewTier, CancellationToken.None, track: true);
        identity.ChangeTier(newTier);
        await _identitiesRepository.Update(identity, CancellationToken.None);
    }
}
