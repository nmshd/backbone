using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers.Base;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Readers;

public class PoolConfigurationJsonReader : PoolConfigurationReaderBase, IPoolConfigurationJsonReader
{
    protected override string[] ValidExtensions { get; } = [".json"];

    public async Task<PerformanceTestConfiguration?> Read(string filePath)
    {
        VerifyFileExtension(filePath);

        var poolConfigFromJsonString = await File.ReadAllTextAsync(filePath);
        var poolConfig = JsonSerializer.Deserialize<PerformanceTestConfiguration>(poolConfigFromJsonString);

        poolConfig?.CreateIdentityPoolConfigurations();

        return poolConfig;
    }
}
