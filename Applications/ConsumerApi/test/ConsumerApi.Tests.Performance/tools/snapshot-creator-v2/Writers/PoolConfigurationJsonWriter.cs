using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;

public class PoolConfigurationJsonWriter
{
    public async Task<(bool Status, string Message)> Write(PerformanceTestConfiguration poolConfigFromExcel, string workSheetName)
    {
        var poolConfigJsonFilePath = Path.Combine(AppContext.BaseDirectory, $"{POOL_CONFIG_JSON_NAME}.{workSheetName}.{JSON_FILE_EXT}");

        try
        {
            var poolConfigJson = JsonSerializer.Serialize(poolConfigFromExcel, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(poolConfigJsonFilePath, poolConfigJson);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }

        return (true, poolConfigJsonFilePath);
    }
}
