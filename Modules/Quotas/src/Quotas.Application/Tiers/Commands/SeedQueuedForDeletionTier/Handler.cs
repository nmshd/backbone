using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.SeedQueuedForDeletionTier;

public class Handler : IRequestHandler<SeedQueuedForDeletionTierCommand>
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

    public async Task Handle(SeedQueuedForDeletionTierCommand request, CancellationToken cancellationToken)
    {
        var queuedForDeletionTier = await _tiersRepository.Find(Tier.QUEUED_FOR_DELETION.Id, CancellationToken.None, true);

        if (queuedForDeletionTier == null)
        {
            queuedForDeletionTier = new Tier(new TierId(Tier.QUEUED_FOR_DELETION.Id), Tier.QUEUED_FOR_DELETION.Name);
            await _tiersRepository.Add(queuedForDeletionTier, CancellationToken.None);
        }

        var metrics = await _metricsRepository.FindAll(CancellationToken.None);
        var addedQuotas = queuedForDeletionTier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics);
        await _tiersRepository.Update(queuedForDeletionTier, CancellationToken.None);

        foreach (var quota in addedQuotas.ToList())
            _eventBus.Publish(new QuotaCreatedForTierIntegrationEvent(queuedForDeletionTier.Id, quota.Id));
    }
}
