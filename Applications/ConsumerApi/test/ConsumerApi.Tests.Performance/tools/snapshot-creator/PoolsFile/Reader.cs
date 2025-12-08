using System.Text.Json;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;

public static class Reader
{
    public static async Task<PoolFileRoot> ReadPools(string poolsFilePath)
    {
        var poolsFile = await File.ReadAllBytesAsync(poolsFilePath);

        var poolsConfiguration = JsonSerializer.Deserialize<PoolFileRoot>(poolsFile, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return poolsConfiguration ?? throw new Exception($"Could not read {poolsFilePath}.");
    }
}
