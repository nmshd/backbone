using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;
public class TierCreatedIntegrationEventHandler : IIntegrationEventHandler<TierCreatedIntegrationEvent>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierCreatedIntegrationEventHandler> _logger;

    public TierCreatedIntegrationEventHandler(ITiersRepository tiersRepository, ILogger<TierCreatedIntegrationEventHandler> logger)
    {
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(TierCreatedIntegrationEvent integrationEvent)
    {
        var tier = new Tier(new TierId(integrationEvent.Id), integrationEvent.Name);
        await _tiersRepository.Add(tier, CancellationToken.None);

        _logger.TierCreated(tier.Id, tier.Name);
    }
}

internal static partial class TierCreatedLogs
{
    [LoggerMessage(
        EventId = 151788,
        EventName = "Quotas.TierCreatedIntegrationEventHandler.TierCreated",
        Level = LogLevel.Information,
        Message = "Successfully created tier. Tier ID: '{tierId}', Tier Name: '{tierName}'.")]
    public static partial void TierCreated(this ILogger logger, TierId tierId, string tierName);
}
