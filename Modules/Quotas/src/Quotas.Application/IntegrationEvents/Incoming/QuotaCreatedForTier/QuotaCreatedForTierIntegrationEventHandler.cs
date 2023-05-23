using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.QuotaCreatedForTier;

public class QuotaCreatedForTierIntegrationEventHandler : IIntegrationEventHandler<QuotaCreatedForTierIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ILogger<QuotaCreatedForTierIntegrationEventHandler> _logger;

    public QuotaCreatedForTierIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ILogger<QuotaCreatedForTierIntegrationEventHandler> logger)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
    }

    public async Task Handle(QuotaCreatedForTierIntegrationEvent @event)
    {
        _logger.LogTrace("Handling QuotaCreatedForTierIntegrationEvent ... ");

        var identitiesWithTier = _identitiesRepository.FindWithTier(@event.TierId).ToList();

        if (identitiesWithTier.Count > 0)
        {
            identitiesWithTier.ForEach(identity =>
            {
                identity.AssignTierQuotaFromDefinition(@event.Quota);
            });

            await _identitiesRepository.Update(identitiesWithTier, CancellationToken.None);

            _logger.LogTrace("Successfully created quotas for Identities!");
        }
        else
        {
            _logger.LogTrace($"No identities found with tier ID: {@event.TierId}");
        }
    }
}
