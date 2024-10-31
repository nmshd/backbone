using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public class PoolConfigurationJsonWriter : IPoolConfigurationJsonWriter
{
    public async Task<StatusMessage> Write(PerformanceTestConfiguration poolConfigFromExcel, string filePath)
    {
        try
        {
            var poolConfigJson = JsonSerializer.Serialize(poolConfigFromExcel, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, poolConfigJson);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message);
        }

        return new StatusMessage(true, Path.GetFullPath(filePath));
    }
}
