using Backbone.Modules.Quotas.Abstractions;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Module.Features.Metrics.Shared;

public class NumberOfCreatedDatawalletModificationsMetricCalculator : IMetricCalculator
{
    private readonly IDatawalletModificationsRepository _datawalletModificationsRepository;

    public NumberOfCreatedDatawalletModificationsMetricCalculator(IDatawalletModificationsRepository datawalletModificationsRepository)
    {
        _datawalletModificationsRepository = datawalletModificationsRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfCreatedDatawalletModifications = await _datawalletModificationsRepository.Count(identityAddress, from, to, cancellationToken);
        return numberOfCreatedDatawalletModifications;
    }
}
