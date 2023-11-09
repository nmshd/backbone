using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.SeedUpForDeletionTier;

public class Handler : IRequestHandler<SeedUpForDeletionTierCommand>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;
    private readonly IEventBus _eventBus;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository, IEventBus eventBus)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
        _eventBus = eventBus;
    }

    public async Task Handle(SeedUpForDeletionTierCommand request, CancellationToken cancellationToken)
    {
        Tier upForDeletionTier;
        try
        {
            upForDeletionTier = await _tiersRepository.Find(TierId.UP_FOR_DELETION_DEFAULT_ID, CancellationToken.None, true);
        }
        catch (NotFoundException)
        {
            upForDeletionTier = new Tier(new TierId(TierId.UP_FOR_DELETION_DEFAULT_ID), Tier.UP_FOR_DELETION_TIER_NAME);
            await _tiersRepository.Add(upForDeletionTier, CancellationToken.None);
        }

        var metrics = await _metricsRepository.FindAll(CancellationToken.None);
        var missingMetrics = metrics.Where(metric => upForDeletionTier.Quotas.All(quota => quota.MetricKey.Value != metric.Key.Value));
        foreach (var metric in missingMetrics)
        {
            var result = upForDeletionTier.CreateQuotaForUpForDeletionTier(metric.Key, 0, QuotaPeriod.Total);
            _eventBus.Publish(new QuotaCreatedForTierIntegrationEvent(upForDeletionTier.Id, result.Value.Id));
        }
        await _tiersRepository.Update(upForDeletionTier, CancellationToken.None);
    }
}
