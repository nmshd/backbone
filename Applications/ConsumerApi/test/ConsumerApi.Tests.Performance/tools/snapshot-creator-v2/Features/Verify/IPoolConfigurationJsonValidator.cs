using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

public interface IPoolConfigurationJsonValidator
{
    Task<bool> Validate(PerformanceTestConfiguration poolConfigJsonFile, PerformanceTestConfiguration poolConfigFromExcel);
}
