using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers.Base;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;

public class PerformanceTestConfigurationJsonReader : PerformanceTestConfigurationReaderBase, IPerformanceTestConfigurationJsonReader
{
    protected override string[] ValidExtensions { get; } = [".json"];

    public async Task<PerformanceTestConfiguration> Read(string filePath)
    {
        VerifyFileExtension(filePath);

        var poolConfigFromJsonString = await File.ReadAllTextAsync(filePath);
        var poolConfig = JsonSerializer.Deserialize<PerformanceTestConfiguration>(poolConfigFromJsonString);

        poolConfig.CreateIdentityPoolConfigurations();

        return poolConfig;
    }
}
