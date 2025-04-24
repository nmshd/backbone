using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.DomainEvents.Incoming.IdentityCreated;
using Backbone.Modules.Quotas.Domain.Metrics;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.DomainEvents.Incoming.IdentityCreated;

public class IdentityCreatedDomainEventHandler : IDomainEventHandler<IdentityCreatedDomainEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;
    private readonly ILogger<IdentityCreatedDomainEventHandler> _logger;

    public IdentityCreatedDomainEventHandler(IIdentitiesRepository identitiesRepository, ILogger<IdentityCreatedDomainEventHandler> logger, ITiersRepository tiersRepository,
        MetricCalculatorFactory metricCalculatorFactory)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _tiersRepository = tiersRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task Handle(IdentityCreatedDomainEvent domainEvent)
    {
        _logger.LogTrace("Handling IdentityCreatedDomainEvent ...");

        var identity = new Identity(domainEvent.Address, TierId.Parse(domainEvent.Tier));

        var tier = await _tiersRepository.Find(identity.TierId, CancellationToken.None, track: true) ?? throw new NotFoundException(nameof(Tier));

        foreach (var tierQuotaDefinition in tier.Quotas)
        {
            identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        }

        await identity.UpdateMetricStatuses(tier.Quotas.Select(q => q.MetricKey), _metricCalculatorFactory, MetricUpdateType.All, CancellationToken.None);

        await _identitiesRepository.Add(identity, CancellationToken.None);

        _logger.IdentityCreated(identity.TierId);
    }
}

internal static partial class IdentityCreatedLogs
{
    [LoggerMessage(
        EventId = 811934,
        EventName = "Quotas.IdentityCreatedDomainEventHandler.IdentityCreated",
        Level = LogLevel.Information,
        Message = "Successfully created the identity. Tier ID: '{tierId}'.")]
    public static partial void IdentityCreated(this ILogger logger, TierId tierId);
}
