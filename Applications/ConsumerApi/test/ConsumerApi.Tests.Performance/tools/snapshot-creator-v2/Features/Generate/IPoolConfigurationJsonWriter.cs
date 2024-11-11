using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public interface IPoolConfigurationJsonWriter
{
    Task<StatusMessage> Write(PerformanceTestConfiguration poolConfigFromExcel, string filePath);
}
