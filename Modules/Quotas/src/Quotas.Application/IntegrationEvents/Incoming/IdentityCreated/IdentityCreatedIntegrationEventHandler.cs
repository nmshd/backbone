using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Quotas.Application.IntegrationEvents.Incoming.IdentityCreated;

public class IdentityCreatedIntegrationEventHandler : IIntegrationEventHandler<IdentityCreatedIntegrationEvent>
{
    private readonly IIdentitiesRepository _identitiesRepository;
    private readonly ITiersRepository _tiersRepository;
    private readonly MetricCalculatorFactory _metricCalculatorFactory;
    private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

    public IdentityCreatedIntegrationEventHandler(IIdentitiesRepository identitiesRepository, ILogger<IdentityCreatedIntegrationEventHandler> logger, ITiersRepository tiersRepository, MetricCalculatorFactory metricCalculatorFactory)
    {
        _identitiesRepository = identitiesRepository;
        _logger = logger;
        _tiersRepository = tiersRepository;
        _metricCalculatorFactory = metricCalculatorFactory;
    }

    public async Task Handle(IdentityCreatedIntegrationEvent integrationEvent)
    {
        _logger.LogTrace("Handling IdentityCreatedIntegrationEvent ...");

        var identity = new Identity(integrationEvent.Address, new TierId(integrationEvent.Tier));

        var tier = await _tiersRepository.Find(identity.TierId, CancellationToken.None, track: true);

        foreach (var tierQuotaDefinition in tier.Quotas)
        {
            identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        }

        await identity.UpdateMetricStatuses(tier.Quotas.Select(q => q.MetricKey), _metricCalculatorFactory, CancellationToken.None);

        await _identitiesRepository.Add(identity, CancellationToken.None);

        _logger.IdentityCreated(identity.Address, identity.TierId);
    }
}

file static class LoggerExtensions
{
    private static readonly Action<ILogger, string, TierId, Exception> IDENTITY_CREATED =
        LoggerMessage.Define<string, TierId>(
            LogLevel.Information,
            new EventId(811934, "IdentityCreatedIntegrationEventHandler.IdentityCreated"),
            "Successfully created identity. Identity Address: '{address}', Tier ID: '{tierId}'."
        );

    public static void IdentityCreated(this ILogger logger, string address, TierId tierId)
    {
        IDENTITY_CREATED(logger, address, tierId, default!);
    }
}
