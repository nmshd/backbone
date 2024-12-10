using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

public class PoolConfigurationJsonValidator : IPoolConfigurationJsonValidator
{
    public Task<bool> Validate(PerformanceTestConfiguration? poolConfigurationFromJson, PerformanceTestConfiguration? poolConfigurationFromExcel)
    {
        var result = poolConfigurationFromJson?.Equals(poolConfigurationFromExcel) ?? false;
        return Task.FromResult(result);
    }
}
