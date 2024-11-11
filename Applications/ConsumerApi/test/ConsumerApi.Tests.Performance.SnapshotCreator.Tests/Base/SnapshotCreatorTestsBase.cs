using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Tests.Base;

public abstract class SnapshotCreatorTestsBase : AbstractTestsBase
{
    protected SnapshotCreatorTestsBase()
    {
        AssertionOptions.AssertEquivalencyUsing(options => options.ComparingRecordsByValue());
    }

    protected string TestDataFolder { get; } = Path.Combine(AppContext.BaseDirectory, "TestData");

    protected async Task<PerformanceTestConfiguration> GetExpectedPoolConfiguration(string expectedPoolConfigJsonFilename)
    {
        var fullFilePath = Path.Combine(TestDataFolder, expectedPoolConfigJsonFilename);

        var poolConfigFromJson = await File.ReadAllTextAsync(fullFilePath);
        var result = JsonSerializer.Deserialize<PerformanceTestConfiguration>(poolConfigFromJson);

        return result;
    }
}
