using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;

public abstract class SnapshotCreatorTestsBase : AbstractTestsBase
{
    protected SnapshotCreatorTestsBase()
    {
        AssertionOptions.AssertEquivalencyUsing(options => options.ComparingRecordsByValue());
    }

    protected string TestDataFolder { get; } = Path.Combine(AppContext.BaseDirectory, "TestData");

    protected async Task<PerformanceTestConfiguration?> GetExpectedPoolConfiguration(string expectedPoolConfigJsonFilename)
    {
        var fullFilePath = Path.Combine(TestDataFolder, expectedPoolConfigJsonFilename);
        var result = await new PoolConfigurationJsonReader().Read(fullFilePath);

        return result;
    }
}
