using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;

namespace Backbone.Modules.Quotas.Application.Metrics;

public class NumberOfStartedDeletionProcessesMetricCalculator : IMetricCalculator
{
    private readonly IIdentityDeletionProcessesRepository _identityDeletionProcessesRepository;

    public NumberOfStartedDeletionProcessesMetricCalculator(IIdentityDeletionProcessesRepository identityDeletionProcessesRepository)
    {
        _identityDeletionProcessesRepository = identityDeletionProcessesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfStartedDeletionProcesses = await _identityDeletionProcessesRepository.CountInStatus(identityAddress, from, to, DeletionProcessStatus.WaitingForApproval, cancellationToken);
        return numberOfStartedDeletionProcesses;
    }
}
