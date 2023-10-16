using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
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

        _logger.TierDeleted(tier.Id, tier.Name);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, TierId, string, Exception> TIER_DELETED =
        LoggerMessage.Define<TierId, string>(
            LogLevel.Information,
            new EventId(582359, "Quotas.TierDeletedIntegrationEventHandler.TierDeleted"),
            "Successfully deleted tier. Tier ID: '{tierId}', Tier Name: '{tierName}'."
        );

    public static void TierDeleted(this ILogger logger, TierId tierId, string name)
    {
        TIER_DELETED(logger, tierId, name, default!);
    }
}
