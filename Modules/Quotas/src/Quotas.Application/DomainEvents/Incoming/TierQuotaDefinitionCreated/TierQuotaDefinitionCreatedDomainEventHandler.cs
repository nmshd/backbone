using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Outgoing;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierQuotaDefinitionCreated;

public class TierQuotaDefinitionCreatedDomainEventHandler : IDomainEventHandler<TierQuotaDefinitionCreatedDomainEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierQuotaDefinitionCreatedDomainEventHandler> _logger;

    public TierQuotaDefinitionCreatedDomainEventHandler(IIdentitiesRepository identitiesRepository,
        ITiersRepository tiersRepository, ILogger<TierQuotaDefinitionCreatedDomainEventHandler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(TierQuotaDefinitionCreatedDomainEvent @event)
    {
        _logger.LogTrace("Handling QuotaCreatedForTierDomainEvent...");

        var identitiesWithTier = await _identitiesRepository.ListWithTier(TierId.Parse(@event.TierId), CancellationToken.None, true);

        if (!identitiesWithTier.Any())
        {
            _logger.LogTrace("No identities found with tier ID: '{tierId}'.", @event.TierId);
            return;
        }

        var tierQuotaDefinition = await _tiersRepository.GetTierQuotaDefinition(@event.TierQuotaDefinitionId, CancellationToken.None, true);

        foreach (var identity in identitiesWithTier)
        {
            identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        }

        await _identitiesRepository.Update(identitiesWithTier, CancellationToken.None);

        _logger.LogInformation("Successfully created quotas for Identities.");

        // This is commented out because it takes several minutes to recalculate the metric statuses.
        // We rather wait for the recalculation to be triggered next time the identity is doing
        // something with the metric. While this means that the metrics status will not be exhausted
        // CAUTION: Remember to comment in the test in TierQuotaDefinitionCreatedDomainEventHandlerTests
        //          in case this gets commented in one day.
        // even if a quota is exhausted, we rather take this risk than having a long-running event handler.
        // var identityAddresses = identitiesWithTier.Select(i => i.Address).ToList();
        // var metrics = new List<MetricKey> { tierQuotaDefinition.MetricKey };
        // await _metricStatusesService.RecalculateMetricStatuses(identityAddresses, metrics, CancellationToken.None);
    }
}
