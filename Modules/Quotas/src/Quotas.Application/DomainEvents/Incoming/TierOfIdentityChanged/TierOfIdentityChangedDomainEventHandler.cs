using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierOfIdentityChanged;
using Backbone.Modules.Quotas.Domain.Metrics;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierOfIdentityChanged;

public class TierOfIdentityChangedDomainEventHandler : IDomainEventHandler<TierOfIdentityChangedDomainEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;

    public TierOfIdentityChangedDomainEventHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task Handle(TierOfIdentityChangedDomainEvent @event)
    {
        var identity = await _identitiesRepository.Get(@event.IdentityAddress, CancellationToken.None, track: true) ?? throw new NotFoundException(nameof(Identity));
        var newTier = await _tiersRepository.Get(@event.NewTierId, CancellationToken.None, track: true) ?? throw new NotFoundException(nameof(Tier));

        await identity.ChangeTier(newTier, _metricCalculatorFactory, CancellationToken.None);

        await _identitiesRepository.Update(identity, CancellationToken.None);
    }
}
