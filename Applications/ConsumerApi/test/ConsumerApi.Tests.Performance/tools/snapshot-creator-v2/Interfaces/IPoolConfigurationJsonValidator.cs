using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

public interface IPoolConfigurationJsonValidator
{
    Task<bool> Validate(PerformanceTestConfiguration poolConfigJsonFile, PerformanceTestConfiguration poolConfigFromExcel);
}
