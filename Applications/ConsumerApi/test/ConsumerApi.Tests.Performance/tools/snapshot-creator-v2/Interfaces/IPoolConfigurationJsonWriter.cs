using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;

public interface IPoolConfigurationJsonWriter
{
    Task<StatusMessage> Write(PerformanceTestConfiguration poolConfigFromExcel, string filePath);
}
