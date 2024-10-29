using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Validators;

public class PoolConfigurationJsonValidator : IPoolConfigurationJsonValidator
{
    public Task<bool> Validate(PerformanceTestConfiguration poolConfigJsonFile, PerformanceTestConfiguration poolConfigFromExcel)
    {
        var result = poolConfigJsonFile.Equals(poolConfigFromExcel);
        return Task.FromResult(result);
    }
}
