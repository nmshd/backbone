using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierDeleted;
public class TierDeletedIntegrationEventHandler : IIntegrationEventHandler<TierDeletedIntegrationEvent>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierDeletedIntegrationEventHandler> _logger;

    public TierDeletedIntegrationEventHandler(ILogger<TierDeletedIntegrationEventHandler> logger, ITiersRepository tiersRepository)
    {
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(TierDeletedIntegrationEvent integrationEvent)
    {
        var tier = await _tiersRepository.Find(integrationEvent.Id, CancellationToken.None);

        await _tiersRepository.RemoveById(tier.Id);

        _logger.LogTrace("Successfully deleted tier. Tier ID: '{tierId}', Tier Name: {tierName}", tier.Id, tier.Name);
    }
}

