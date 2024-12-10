using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Verify;

public interface IPoolConfigurationJsonValidator
{
    Task<bool> Validate(PerformanceTestConfiguration? poolConfigurationFromJson, PerformanceTestConfiguration? poolConfigurationFromExcel);
}
