using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.QuotaCreatedForTier;

public class QuotaCreatedForTierIntegrationEventHandler : IIntegrationEventHandler<QuotaCreatedForTierIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITierQuotaDefinitionsRepository _tierQuotaDefinitionsRepository;
    private readonly ILogger<QuotaCreatedForTierIntegrationEventHandler> _logger;

    public QuotaCreatedForTierIntegrationEventHandler(IIdentitiesRepository identitiesRepository,
        ITierQuotaDefinitionsRepository tierQuotaDefinitionsRepository, ILogger<QuotaCreatedForTierIntegrationEventHandler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _tierQuotaDefinitionsRepository = tierQuotaDefinitionsRepository;
        _logger = logger;
    }

    public async Task Handle(QuotaCreatedForTierIntegrationEvent @event)
    {
        _logger.LogTrace("Handling QuotaCreatedForTierIntegrationEvent ... ");

        var identitiesWithTier = _identitiesRepository.FindWithTier(@event.TierId).ToList();

        if (identitiesWithTier.Count == 0)
        {
            _logger.LogTrace($"No identities found with tier ID: {@event.TierId}");
            return;
        }

        var tierQuotaDefinition = await _tierQuotaDefinitionsRepository.Find(@event.TierQuotaDefinitionId, CancellationToken.None);

        foreach (var identity in identitiesWithTier)
        {
            identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        }

        await _identitiesRepository.Update(identitiesWithTier, CancellationToken.None);

        _logger.LogTrace("Successfully created quotas for Identities!");
    }
}
