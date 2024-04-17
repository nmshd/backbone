using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierCreated;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.TierCreated;

public class TierCreatedDomainEventHandler : IDomainEventHandler<TierCreatedDomainEvent>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierCreatedDomainEventHandler> _logger;

    public TierCreatedDomainEventHandler(ITiersRepository tiersRepository, ILogger<TierCreatedDomainEventHandler> logger)
    {
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(TierCreatedDomainEvent domainEvent)
    {
        var tier = new Tier(new TierId(domainEvent.Id), domainEvent.Name);
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
