using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
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


        List<IdentityPoolConfiguration> identityPoolConfigs = [];
        Configuration configuration = new();

        var performanceTestConfiguration = new PerformanceTestConfiguration(identityPoolConfigs, configuration);

        var materializedPoolConfig = poolConfigFromExcel as dynamic[] ?? poolConfigFromExcel.ToArray();
        if (materializedPoolConfig.Length == 0)
        {
            throw new InvalidOperationException(PERFORMANCE_TEST_CONFIGURATION_EXCEL_FILE_EMPTY);
        }

        if (materializedPoolConfig.First() is not IDictionary<string, object> firstRow)
        {
            throw new InvalidOperationException($"{PERFORMANCE_TEST_CONFIGURATION_FIRST_ROW_MISMATCH} {nameof(IDictionary<string, object>)}");
        }

        configuration.App = new AppConfig
        {
            TotalNumberOfSentMessages = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_SENT_MESSAGES]),
            TotalNumberOfReceivedMessages = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_RECEIVED_MESSAGES]),
            NumberOfReceivedMessagesAddOn = Convert.ToInt64(firstRow[APP_NUMBER_OF_RECEIVED_MESSAGES_ADD_ON]),
            TotalNumberOfRelationships = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_RELATIONSHIPS])
        };
        configuration.Connector = new ConnectorConfig
        {
            TotalNumberOfSentMessages = Convert.ToInt64(firstRow[CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES]),
            TotalNumberOfReceivedMessages = Convert.ToInt64(firstRow[CONNECTOR_TOTAL_NUMBER_OF_RECEIVED_MESSAGES]),
            NumberOfReceivedMessagesAddOn = Convert.ToInt64(firstRow[CONNECTOR_NUMBER_OF_RECEIVED_MESSAGES_ADD_ON]),
            TotalNumberOfAvailableRelationships = Convert.ToInt64(firstRow[CONNECTOR_TOTAL_NUMBER_OF_AVAILABLE_RELATIONSHIPS])
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
                NumberOfRelationshipTemplates = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfRelationshipTemplates)]),
                NumberOfRelationships = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfRelationships)]),
                NumberOfSentMessages = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfSentMessages)]),
                NumberOfReceivedMessages = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfReceivedMessages)]),
                NumberOfDatawalletModifications = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfDatawalletModifications)]),
                NumberOfDevices = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfDevices)]),
                NumberOfChallenges = Convert.ToInt32(row[nameof(IdentityPoolConfiguration.NumberOfChallenges)])
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
            return (false, INVALID_FILE_PATH);
        }

        var poolConfigJsonFilePath = Path.Combine(filePath, $"{POOL_CONFIG_JSON_NAME}.{workSheetName}.{POOL_CONFIG_JSON_EXT}");

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

    public async Task<(bool Status, string Message)> GenerateExcelRelationshipsAndMessagesPoolConfig(string poolConfigJsonFile, string workSheetName)
    {
        var savePath = Path.GetDirectoryName(poolConfigJsonFile);

        if (string.IsNullOrWhiteSpace(savePath))
        {
            return (false, INVALID_FILE_PATH);
        }

        var excelFilePath = Path.Combine(savePath, $"{RELATIONSHIPS_AND_MESSAGE_POOL_CONFIGS_FILE_NAME}.{workSheetName}.{EXCEL_FILE_EXT}");

        try
        {
            #region Deserialize Pool Config and create a list of IdentityPools from it

            var poolConfig = await DeserializeFromJson(poolConfigJsonFile);

            var identityPools = poolConfig.IdentityPoolConfigs
                .Select(poolConfigIdentityPoolConfig => new IdentityPool(poolConfigIdentityPoolConfig))
                .ToArray();

            #endregion

            #region Iterate over each App IdentityPool and App Identity and build a relationship to an available connector Identity

            var appIdentityPools = identityPools.Where(ip => ip.Type == IdentityPoolType.App).ToArray();
            var connectorIdentityPools = identityPools.Where(ip => ip.Type == IdentityPoolType.Connector).ToArray();

            foreach (var appIdentityPool in appIdentityPools)
            {
                // Iterate over each App Identity in that App Pool and find a Connector Identity with available relationships
                foreach (var appIdentity in appIdentityPool.Identities)
                {
                    while (appIdentity.HasAvailableRelationships)
                    {
                        // Find a connector identity with available relationships in all connector identity pools 
                        var connectorIdentity = connectorIdentityPools
                                                    .SelectMany(cip => cip.Identities)
                                                    .FirstOrDefault(i => i.HasAvailableRelationships)
                                                ?? throw new InvalidOperationException(CONNECTOR_NO_MORE_IDENTITIES_AVAILABLE);

                        // Add relationship between app identity and connector identity (forward relationship)
                        appIdentity.RelationshipAndMessages.Add(
                            new RelationshipAndMessages(
                                SenderPool: appIdentity.PoolAlias,
                                SenderIdentityId: appIdentity.Id,
                                ReceiverPool: connectorIdentity.PoolAlias,
                                ReceiverIdentityId: connectorIdentity.Id,
                                ReceiverIdentity: connectorIdentity)
                        );

                        // Decrement available relationships of the app identity
                        appIdentity.DecrementAvailableRelationships();

                        appIdentity.ConfigureMessagesSentTo(connectorIdentity);

                        // Add relationship between connector identity and app identity (reverse relationship)
                        connectorIdentity.RelationshipAndMessages.Add(
                            new RelationshipAndMessages(
                                SenderPool: connectorIdentity.PoolAlias,
                                SenderIdentityId: connectorIdentity.Id,
                                ReceiverPool: appIdentity.PoolAlias,
                                ReceiverIdentityId: appIdentity.Id,
                                ReceiverIdentity: appIdentity)
                        );

                        // Decrement available relationships of the connector identity
                        connectorIdentity.DecrementAvailableRelationships();

                        connectorIdentity.ConfigureMessagesSentTo(appIdentity);
                    }
                }
            }

            #endregion

            #region Save Relationships and Messages to Excel

            var relationshipAndMessagesList =
                identityPools.SelectMany(ipr => ipr.Identities.SelectMany(i => i.RelationshipAndMessages));

            await new ExcelMapper().SaveAsync(excelFilePath, relationshipAndMessagesList, workSheetName);

            #endregion
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }

        return (true, excelFilePath);
    }
}
