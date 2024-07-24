using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;

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
