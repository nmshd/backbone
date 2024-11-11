using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;

public interface IPoolConfigurationExcelReader
{
    Task<PerformanceTestConfiguration> Read(string filePath, string workSheet);
}
