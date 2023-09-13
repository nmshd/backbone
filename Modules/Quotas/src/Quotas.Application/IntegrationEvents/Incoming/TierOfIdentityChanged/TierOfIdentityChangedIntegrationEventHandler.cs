using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierOfIdentityChanged;
public class TierOfIdentityChangedIntegrationEventHandler : IIntegrationEventHandler<TierOfIdentityChangedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricStatusesService _metricStatusesService;

    public TierOfIdentityChangedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ITiersRepository tiersRepository, IMetricStatusesService metricStatusesService)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _metricStatusesService = metricStatusesService;
    }

    public async Task Handle(TierOfIdentityChangedIntegrationEvent @event)
    {
        var identity = await _identitiesRepository.Find(@event.IdentityAddress, CancellationToken.None, track: true);
        var newTier = await _tiersRepository.Find(@event.NewTier, CancellationToken.None, track: true);
        
        identity.ChangeTier(newTier);
        
        await _identitiesRepository.Update(identity, CancellationToken.None);

        await _metricStatusesService.RecalculateMetricStatuses(
            new List<string>() { identity.Address },
            identity.MetricStatuses.Select(x => x.MetricKey).ToList(),
            CancellationToken.None
        );
    }
}
