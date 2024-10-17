using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;

public class IdentityPoolConfigGenerator
{
    public async Task<bool> VerifyJsonPoolConfig(string excelFile, string worksheet, string poolConfigJsonFile)
    {
        #region Deserialize json into Object instance

        var poolConfigFromJson = await DeserializeFromJson(poolConfigJsonFile);

        #endregion


        #region Deserialize Excel into Object instance

        var poolConfigFromExcel = await DeserializeFromExcel(excelFile);

        #endregion

        #region Verify Equality

        var result = poolConfigFromJson?.Equals(poolConfigFromExcel) ?? false;

        #endregion

        return result;
    }

    internal async Task<PerformanceTestConfiguration?> DeserializeFromJson(string poolConfigJsonFile)
    {
        var poolConfigFromJsonString = await File.ReadAllTextAsync(poolConfigJsonFile);
        var poolConfig = JsonSerializer.Deserialize<PerformanceTestConfiguration>(poolConfigFromJsonString);

        return poolConfig;
    }

    private Task<PerformanceTestConfiguration> DeserializeFromExcel(string excelFile)
    {
        var excelMapper = new ExcelMapper(excelFile) { SkipBlankRows = true, SkipBlankCells = true, TrackObjects = false };
        var poolConfigFromExcel = excelMapper.Fetch();
        
        //TODO: Map poolConfigFromExcel to PoolConfig

        foreach (var data in poolConfigFromExcel)
        {
            System.Diagnostics.Debug.WriteLine($"data.Type: {data.Type}");
        }


        List<IdentityPoolConfiguration> identityPoolConfigs  =new();
        Configuration configuration = new();

        var poolConfig = new PerformanceTestConfiguration(identityPoolConfigs, configuration);

        return Task.FromResult(poolConfig);
    }
}
