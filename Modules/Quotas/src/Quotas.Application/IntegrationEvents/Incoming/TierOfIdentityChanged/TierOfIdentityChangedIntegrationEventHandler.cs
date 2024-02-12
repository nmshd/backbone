using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEventHandler : IIntegrationEventHandler<TierOfIdentityChangedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public TierOfIdentityChangedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task Handle(TierOfIdentityChangedIntegrationEvent @event)
    {
        var identity = await _identitiesRepository.Find(@event.IdentityAddress, CancellationToken.None, track: true) ?? throw new NotFoundException(nameof(Identity));
        var newTier = await _tiersRepository.Find(@event.NewTier, CancellationToken.None, track: true) ?? throw new NotFoundException(nameof(Tier));

        await identity.ChangeTier(newTier, _metricCalculatorFactory, CancellationToken.None);

        await _identitiesRepository.Update(identity, CancellationToken.None);
    }
}
