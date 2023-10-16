using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
public class TierCreatedIntegrationEventHandler : IIntegrationEventHandler<TierCreatedIntegrationEvent>
{
    private readonly ITiersRepository _tierRepository;
    private readonly ILogger<TierCreatedIntegrationEventHandler> _logger;

    public TierCreatedIntegrationEventHandler(ITiersRepository tierRepository, ILogger<TierCreatedIntegrationEventHandler> logger)
    {
        _tierRepository = tierRepository;
        _logger = logger;
    }

    public async Task Handle(TierCreatedIntegrationEvent integrationEvent)
    {
        var tier = new Tier(new TierId(integrationEvent.Id), integrationEvent.Name);
        await _tierRepository.Add(tier, CancellationToken.None);

        _logger.TierCreated(tier.Id, tier.Name);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, TierId, string, Exception> TIER_CREATED =
        LoggerMessage.Define<TierId, string>(
            LogLevel.Information,
            new EventId(151788, "Quotas.TierCreatedIntegrationEventHandler.TierCreated"),
            "Successfully created tier. Tier ID: '{tierId}', Tier Name: '{tierName}'."
        );

    public static void TierCreated(this ILogger logger, TierId tierId, string name)
    {
        TIER_CREATED(logger, tierId, name, default!);
    }
}
