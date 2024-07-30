using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
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
        const int checkInterval = 5000;
        const int maxRetries = 5;

        var retries = 0;
        var tierFound = false;

        while (!tierFound && retries < maxRetries)
        {
            retries++;

            var queuedForDeletionTier = await _tiersRepository.Find(Tier.QUEUED_FOR_DELETION.Id, CancellationToken.None, true);

            if (queuedForDeletionTier != null)
            {
                tierFound = true;

                var metrics = await _metricsRepository.FindAll(CancellationToken.None);
                var excludedMetricKeys = new List<MetricKey>
                {
                    MetricKey.NumberOfCreatedDatawalletModifications, // Identities to be deleted should still be able to modify the datawallet
                    MetricKey.NumberOfStartedDeletionProcesses // Identities to be deleted cannot start new deletion processes anyway
                };
                queuedForDeletionTier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics.Where(m => !excludedMetricKeys.Contains(m.Key)));
                await _tiersRepository.Update(queuedForDeletionTier, CancellationToken.None);
            }
            else
            {
                await Task.Delay(checkInterval, cancellationToken);
            }
        }
    }
}
