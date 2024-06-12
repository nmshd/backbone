using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;
public class NumberOfStartedDeletionProcessesMetricCalculator : IMetricCalculator
{
    private readonly IStartedDeletionProcessesRepository _startedDeletionProcessesRepository;

    public NumberOfStartedDeletionProcessesMetricCalculator(IStartedDeletionProcessesRepository startedDeletionProcessesRepository)
    {
        _startedDeletionProcessesRepository = startedDeletionProcessesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfStartedDeletionProcesses = await _startedDeletionProcessesRepository.Count(identityAddress, from, to, cancellationToken);
        return numberOfStartedDeletionProcesses;
    }
}
