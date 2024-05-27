using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using MediatR;

namespace Backbone.Modules.Quotas.Application.Tiers.Commands.SeedQueuedForDeletionTier;

public class Handler : IRequestHandler<SeedQueuedForDeletionTierCommand>
{
    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task Handle(SeedQueuedForDeletionTierCommand request, CancellationToken cancellationToken)
    {
        var queuedForDeletionTier = await _tiersRepository.Find(Tier.QUEUED_FOR_DELETION.Id, CancellationToken.None, true);

        if (queuedForDeletionTier == null)
        {
            queuedForDeletionTier = new Tier(Tier.QUEUED_FOR_DELETION.Id, Tier.QUEUED_FOR_DELETION.Name);
            await _tiersRepository.Add(queuedForDeletionTier, CancellationToken.None);
        }

        var metrics = await _metricsRepository.FindAll(CancellationToken.None);
        queuedForDeletionTier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics);
        await _tiersRepository.Update(queuedForDeletionTier, CancellationToken.None);
    }
}
