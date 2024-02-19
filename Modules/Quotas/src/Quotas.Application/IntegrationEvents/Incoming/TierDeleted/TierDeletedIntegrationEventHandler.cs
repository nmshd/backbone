using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
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
        var tier = await _tiersRepository.Find(integrationEvent.Id, CancellationToken.None) ?? throw new NotFoundException(nameof(Tier));

        await _tiersRepository.RemoveById(tier.Id);

        _logger.TierDeleted(tier.Id, tier.Name);
    }
}

internal static partial class TierDeletedLogs
{
    [LoggerMessage(
        EventId = 582359,
        EventName = "Quotas.TierDeletedIntegrationEventHandler.TierDeleted",
        Level = LogLevel.Information,
        Message = "Successfully deleted tier. Tier ID: '{tierId}', Tier Name: '{tierName}'.")]
    public static partial void TierDeleted(this ILogger logger, TierId tierId, string tierName);
}
