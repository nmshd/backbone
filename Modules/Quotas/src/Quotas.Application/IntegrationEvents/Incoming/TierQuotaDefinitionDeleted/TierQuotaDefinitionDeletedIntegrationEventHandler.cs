using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierQuotaDefinitionDeleted;
public class TierQuotaDefinitionDeletedIntegrationEventHandler : IIntegrationEventHandler<TierQuotaDefinitionDeletedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierQuotaDefinitionDeletedIntegrationEventHandler> _logger;

    public TierQuotaDefinitionDeletedIntegrationEventHandler(IIdentitiesRepository identitiesRepository,
        ITiersRepository tiersRepository, ILogger<TierQuotaDefinitionDeletedIntegrationEventHandler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(TierQuotaDefinitionDeletedIntegrationEvent @event)
    {
        _logger.LogTrace("Handling TierQuotaDefinitionDeletedIntegrationEvent ... ");

        var identitiesWithTier = await _identitiesRepository.FindWithTier(new TierId(@event.TierId), CancellationToken.None, true);

        if (!identitiesWithTier.Any())
        {
            _logger.LogTrace($"No identities found with tier ID: {@event.TierId}");
            return;
        }

        var tierQuotaDefinition = await _tiersRepository.FindTierQuotaDefinition(@event.TierQuotaDefinitionId, CancellationToken.None, true);

        foreach (var identity in identitiesWithTier)
        {
            identity.DeleteTierQuotaFromDefinitionId(tierQuotaDefinition.Id);
        }

        await _identitiesRepository.Update(identitiesWithTier, CancellationToken.None);

        _logger.LogTrace("Successfully deleted quotas for Identities!");
    }
}
