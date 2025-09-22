using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.TierDeleted;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.TierDeleted;

public class TierDeletedDomainEventHandler : IDomainEventHandler<TierDeletedDomainEvent>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly ILogger<TierDeletedDomainEventHandler> _logger;

    public TierDeletedDomainEventHandler(ILogger<TierDeletedDomainEventHandler> logger, ITiersRepository tiersRepository)
    {
        _tiersRepository = tiersRepository;
        _logger = logger;
    }

    public async Task Handle(TierDeletedDomainEvent domainEvent)
    {
        var tier = await _tiersRepository.Get(domainEvent.Id, CancellationToken.None) ?? throw new NotFoundException(nameof(Tier));

        await _tiersRepository.RemoveById(tier.Id);

        _logger.TierDeleted(tier.Id, tier.Name);
    }
}

internal static partial class TierDeletedLogs
{
    [LoggerMessage(
        EventId = 582359,
        EventName = "Quotas.TierDeletedDomainEventHandler.TierDeleted",
        Level = LogLevel.Information,
        Message = "Successfully deleted tier. Tier ID: '{tierId}', Tier Name: '{tierName}'.")]
    public static partial void TierDeleted(this ILogger logger, TierId tierId, string tierName);
}
