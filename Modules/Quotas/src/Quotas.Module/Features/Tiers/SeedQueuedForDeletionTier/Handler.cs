using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain.Aggregates.Metrics;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Tooling.Extensions;
using MediatR;

namespace Backbone.Modules.Quotas.Module.Features.Tiers.SeedQueuedForDeletionTier;

public class Handler : IRequestHandler<Command>
{
    private const int MAX_RETRIES_TO_FIND_QUEUED_FOR_DELETION_TIER = 5;
    private static readonly TimeSpan RETRY_DELAY = 5.Seconds();

    private readonly ITiersRepository _tiersRepository;
    private readonly IMetricsRepository _metricsRepository;

    public Handler(ITiersRepository tiersRepository, IMetricsRepository metricsRepository)
    {
        _tiersRepository = tiersRepository;
        _metricsRepository = metricsRepository;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var queuedForDeletionTier = await ReadQueuedForDeletionTier(cancellationToken) ?? throw new Exception("Queued for deletion tier not found");
        await Seed(queuedForDeletionTier);

        await _tiersRepository.Update(queuedForDeletionTier, CancellationToken.None);
    }

    private async Task<Tier?> ReadQueuedForDeletionTier(CancellationToken cancellationToken)
    {
        for (var retries = 0; retries < MAX_RETRIES_TO_FIND_QUEUED_FOR_DELETION_TIER; retries++)
        {
            var queuedForDeletionTier = await _tiersRepository.Get(Tier.QUEUED_FOR_DELETION.Id, CancellationToken.None, true);

            if (queuedForDeletionTier != null)
                return queuedForDeletionTier;

            await Task.Delay(RETRY_DELAY, cancellationToken);
        }

        return null;
    }

    private async Task Seed(Tier queuedForDeletionTier)
    {
        var metrics = await _metricsRepository.List(CancellationToken.None);
        var excludedMetricKeys = new List<MetricKey>
        {
            MetricKey.NUMBER_OF_CREATED_DATAWALLET_MODIFICATIONS, // Identities to be deleted should still be able to modify the datawallet
            MetricKey.NUMBER_OF_STARTED_DELETION_PROCESSES // Identities to be deleted cannot start new deletion processes anyway
        };
        queuedForDeletionTier.AddQuotaForAllMetricsOnQueuedForDeletion(metrics.Where(m => !excludedMetricKeys.Contains(m.Key)));
    }
}
