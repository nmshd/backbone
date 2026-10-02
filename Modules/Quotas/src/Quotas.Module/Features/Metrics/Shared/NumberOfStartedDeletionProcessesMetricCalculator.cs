using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.Shared;

public class NumberOfStartedDeletionProcessesMetricCalculator : IMetricCalculator
{
    private readonly IIdentityDeletionProcessesRepository _identityDeletionProcessesRepository;

    public NumberOfStartedDeletionProcessesMetricCalculator(IIdentityDeletionProcessesRepository identityDeletionProcessesRepository)
    {
        _identityDeletionProcessesRepository = identityDeletionProcessesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfStartedDeletionProcesses = await _identityDeletionProcessesRepository.CountInStatus(identityAddress, from, to, cancellationToken);
        return numberOfStartedDeletionProcesses;
    }
}
