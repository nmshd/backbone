using Backbone.Modules.Quotas.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Quotas.Domain;

namespace Backbone.Modules.Quotas.Application.Metrics;

public class NumberOfCreatedDevicesMetricCalculator : IMetricCalculator
{
    private readonly IDevicesRepository _devicesRepository;

    public NumberOfCreatedDevicesMetricCalculator(IDevicesRepository devicesRepository)
    {
        _devicesRepository = devicesRepository;
    }

    public async Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        var numberOfCreatedDevices = await _devicesRepository.Count(identityAddress, from, to, cancellationToken);
        return numberOfCreatedDevices;
    }
}
