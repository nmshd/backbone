using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Ganss.Excel;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2;

public class IdentityPoolConfigGenerator : IIdentityPoolConfigGenerator
{
    internal static async Task<PerformanceTestConfiguration> DeserializeFromJson(string poolConfigJsonFile)
    {
        var poolConfigFromJsonString = await File.ReadAllTextAsync(poolConfigJsonFile);
        var poolConfig = JsonSerializer.Deserialize<PerformanceTestConfiguration>(poolConfigFromJsonString);

        return poolConfig;
    }

    internal static async Task<PerformanceTestConfiguration> DeserializeFromExcel(string excelFile, string workSheet)
    {
        var excelMapper = new ExcelMapper(excelFile) { SkipBlankRows = true, SkipBlankCells = true, TrackObjects = false };

        await using var stream = new FileStream(excelFile, FileMode.Open, FileAccess.Read);
        var poolConfigFromExcel = await excelMapper.FetchAsync(stream, workSheet);


        List<IdentityPoolConfiguration> identityPoolConfigs = new();
        Configuration configuration = new();

        var performanceTestConfiguration = new PerformanceTestConfiguration(identityPoolConfigs, configuration);

        var materializedPoolConfig = poolConfigFromExcel as dynamic[] ?? poolConfigFromExcel.ToArray();
        if (materializedPoolConfig.Length == 0)
        {
            throw new InvalidOperationException("Excel file is empty");
        }

        if (materializedPoolConfig.First() is not IDictionary<string, object> firstRow)
        {
            throw new InvalidOperationException("First row is not of type IDictionary<string, object>");
        }

        configuration.App = new AppConfig
        {
            TotalNumberOfSentMessages = Convert.ToInt64(firstRow["App.TotalNumberOfSentMessages"]),
            TotalNumberOfReceivedMessages = Convert.ToInt64(firstRow["App.TotalNumberOfReceivedMessages"]),
            NumberOfReceivedMessagesAddOn = Convert.ToInt64(firstRow["App.NumberOfReceivedMessagesAddOn"]),
            TotalNumberOfRelationships = Convert.ToInt64(firstRow["App.TotalNumberOfRelationships"])
        };
        configuration.Connector = new ConnectorConfig
        {
            TotalNumberOfSentMessages = Convert.ToInt64(firstRow["Connector.TotalNumberOfSentMessages"]),
            TotalNumberOfReceivedMessages = Convert.ToInt64(firstRow["Connector.TotalNumberOfReceivedMessages"]),
            NumberOfReceivedMessagesAddOn = Convert.ToInt64(firstRow["Connector.NumberOfReceivedMessagesAddOn"]),
            TotalNumberOfAvailableRelationships = Convert.ToInt64(firstRow["Connector.TotalNumberOfAvailableRelationships"])
        };

        foreach (var data in materializedPoolConfig)
        {
            if (data is not IDictionary<string, object> row)
            {
                continue;
            }

            var item = new IdentityPoolConfiguration
            {
                Type = (string)row[nameof(IdentityPoolConfiguration.Type)],
                Name = (string)row[nameof(IdentityPoolConfiguration.Name)],
                Alias = (string)row[nameof(IdentityPoolConfiguration.Alias)],
                Amount = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.Amount)]),
                NumberOfRelationshipTemplates = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfRelationshipTemplates)]),
                NumberOfRelationships = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfRelationships)]),
                NumberOfSentMessages = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfSentMessages)]),
                NumberOfReceivedMessages = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfReceivedMessages)]),
                NumberOfDatawalletModifications = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfDatawalletModifications)]),
                NumberOfDevices = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfDevices)]),
                NumberOfChallenges = Convert.ToInt64(row[nameof(IdentityPoolConfiguration.NumberOfChallenges)])
            };

            identityPoolConfigs.Add(item);
        }

        return performanceTestConfiguration;
    }


    public async Task<bool> VerifyJsonPoolConfig(string excelFile, string workSheetName, string poolConfigJsonFile)
    {
        var poolConfigFromJson = await DeserializeFromJson(poolConfigJsonFile);
        var poolConfigFromExcel = await DeserializeFromExcel(excelFile, workSheetName);
        var result = poolConfigFromJson.Equals(poolConfigFromExcel);

        return result;
    }

    public async Task<(bool Status, string Message)> GenerateJsonPoolConfig(string excelFile, string workSheetName)
    {
        var filePath = Path.GetDirectoryName(excelFile);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return (false, "Invalid file path");
        }

        var poolConfigJsonFilePath = Path.Combine(filePath, $"pool-config.{workSheetName}.json");

        try
        {
            var poolConfigFromExcel = await DeserializeFromExcel(excelFile, workSheetName);
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
