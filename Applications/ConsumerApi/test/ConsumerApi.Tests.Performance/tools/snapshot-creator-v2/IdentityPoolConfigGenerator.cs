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
        VerificationConfiguration verificationConfiguration = new();

        var performanceTestConfiguration = new PerformanceTestConfiguration(identityPoolConfigs, verificationConfiguration);

        var materializedPoolConfig = poolConfigFromExcel as dynamic[] ?? poolConfigFromExcel.ToArray();
        if (materializedPoolConfig.Length == 0)
        {
            throw new InvalidOperationException(PERFORMANCE_TEST_CONFIGURATION_EXCEL_FILE_EMPTY);
        }

        if (materializedPoolConfig.First() is not IDictionary<string, object> firstRow)
        {
            throw new InvalidOperationException($"{PERFORMANCE_TEST_CONFIGURATION_FIRST_ROW_MISMATCH} {nameof(IDictionary<string, object>)}");
        }

        verificationConfiguration.App = new AppVerificationConfiguration
        {
            TotalNumberOfSentMessages = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_SENT_MESSAGES]),
            TotalNumberOfReceivedMessages = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_RECEIVED_MESSAGES]),
            NumberOfReceivedMessagesAddOn = Convert.ToInt32(firstRow[APP_NUMBER_OF_RECEIVED_MESSAGES_ADD_ON]),
            TotalNumberOfRelationships = Convert.ToInt64(firstRow[APP_TOTAL_NUMBER_OF_RELATIONSHIPS])
        };
        verificationConfiguration.Connector = new ConnectorVerificationConfiguration
        {
            TotalNumberOfSentMessages = Convert.ToInt64(firstRow[CONNECTOR_TOTAL_NUMBER_OF_SENT_MESSAGES]),
            TotalNumberOfReceivedMessages = Convert.ToInt64(firstRow[CONNECTOR_TOTAL_NUMBER_OF_RECEIVED_MESSAGES]),
            NumberOfReceivedMessagesAddOn = Convert.ToInt32(firstRow[CONNECTOR_NUMBER_OF_RECEIVED_MESSAGES_ADD_ON]),
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

        var poolConfigJsonFilePath = Path.Combine(filePath, $"{POOL_CONFIG_JSON_NAME}.{workSheetName}.{JSON_FILE_EXT}");

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
                        var connectorIdentities = connectorIdentityPools
                            .SelectMany(cip => cip.Identities)
                            .Where(i => i.HasAvailableRelationships)
                            .ToArray();


                        Identity receiverConnectorIdentity = null;
                        foreach (var connectorIdentity in connectorIdentities)
                        {
                            var hasAppIdentityaConnectorIdentityRelationship = appIdentity.RelationshipAndMessages.Any(rm => rm.ReceiverIdentityAddress == connectorIdentity.Address);
                            var hasConnectorIdentityanAppIdentityRelationship = connectorIdentity.RelationshipAndMessages.Any(rm => rm.ReceiverIdentityAddress == appIdentity.Address);

                            if (hasAppIdentityaConnectorIdentityRelationship || hasConnectorIdentityanAppIdentityRelationship)
                            {
                                continue;
                            }

                            if (!connectorIdentity.HasAvailableRelationships)
                            {
                                continue;
                            }

                            receiverConnectorIdentity = connectorIdentity;
                            break;
                        }

                        if (receiverConnectorIdentity == null)
                        {
                            receiverConnectorIdentity = connectorIdentities.FirstOrDefault(c => c.HasAvailableRelationships);

                            if (receiverConnectorIdentity == null)
                            {
                                throw new InvalidOperationException(string.Format(RELATIONSHIP_NO_RECEIVER_AVAILABLE, appIdentity.Address, appIdentity.PoolAlias));
                            }
                        }

                        // Add relationship between app identity and connector identity (forward relationship)
                        var relationshipAndMessages = new RelationshipAndMessages(
                            SenderPool: appIdentity.PoolAlias,
                            SenderIdentityAddress: appIdentity.Address,
                            ReceiverPool: receiverConnectorIdentity.PoolAlias,
                            ReceiverIdentityAddress: receiverConnectorIdentity.Address,
                            ReceiverIdentityPoolType: receiverConnectorIdentity.IdentityPoolType);

                        appIdentity.RelationshipAndMessages.Add(relationshipAndMessages);

                        // Decrement available relationships of the app identity
                        appIdentity.DecrementAvailableRelationships();

                        relationshipAndMessages.NumberOfSentMessages = appIdentity.HasAvailableRelationships
                            ? appIdentity.MessagesToSendPerRelationship
                            : appIdentity.MessagesToSendPerRelationship + appIdentity.ModuloSendMessages;


                        // Add relationship between connector identity and app identity (reverse relationship)
                        var reverseRelationshipAndMessages = new RelationshipAndMessages(
                            SenderPool: receiverConnectorIdentity.PoolAlias,
                            SenderIdentityAddress: receiverConnectorIdentity.Address,
                            ReceiverPool: appIdentity.PoolAlias,
                            ReceiverIdentityAddress: appIdentity.Address,
                            ReceiverIdentityPoolType: appIdentity.IdentityPoolType);

                        receiverConnectorIdentity.RelationshipAndMessages.Add(reverseRelationshipAndMessages);

                        // Decrement available relationships of the connector identity
                        receiverConnectorIdentity.DecrementAvailableRelationships();

                        reverseRelationshipAndMessages.NumberOfSentMessages = receiverConnectorIdentity.HasAvailableRelationships
                            ? receiverConnectorIdentity.MessagesToSendPerRelationship
                            : receiverConnectorIdentity.MessagesToSendPerRelationship + receiverConnectorIdentity.ModuloSendMessages;
                    }
                }
            }

            #endregion

            var relationshipAndMessagesList =
                identityPools.SelectMany(ipr => ipr.Identities.SelectMany(i => i.RelationshipAndMessages)).ToArray();

            var relationShipCount = relationshipAndMessagesList.Length / 2; // Note: Div by 2 because a pair of forward/reverse relationship is equal to 1 relationship

            if (relationShipCount != poolConfig.VerificationConfiguration.App.TotalNumberOfRelationships)
            {
                throw new InvalidOperationException(string.Format(RELATIONSHIP_COUNT_MISMATCH, poolConfig.VerificationConfiguration.App.TotalNumberOfRelationships, relationShipCount));
            }

            void VerifyAndFixNumberOfSentMessages(RelationshipAndMessages[] relationshipAndMessages, IdentityPoolType identityPoolType, int expectedTotalNumberOfSentMessages)
            {
                var filteredRelationships = relationshipAndMessages.Where(rm => rm.ReceiverIdentityPoolType == identityPoolType).ToList();
                var actualTotalNumberOfSentMessages = filteredRelationships.Sum(rm => rm.NumberOfSentMessages);

                if (actualTotalNumberOfSentMessages == expectedTotalNumberOfSentMessages) return;

                var messageDifference = expectedTotalNumberOfSentMessages - actualTotalNumberOfSentMessages;

                if (messageDifference == 0) return;

                var correctionMessages = messageDifference / filteredRelationships.Count;

                foreach (var relationship in filteredRelationships)
                {
                    relationship.NumberOfSentMessages += correctionMessages;
                }

                var correctionModulo = messageDifference % filteredRelationships.Count;

                if (correctionModulo == 0) return;

                var lastRelationship = filteredRelationships.Last();
                lastRelationship.NumberOfSentMessages += correctionModulo;
            }

            #region Verify Sent Messages against the Pool VerificationConfiguration were ConnectorPool Idenities are the receivers

            VerifyAndFixNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.Connector, (int)poolConfig.VerificationConfiguration.App.TotalNumberOfSentMessages);

            #endregion

            #region Verify Sent Messages against the Pool VerificationConfiguration were AppPool Identities are the receivers

            VerifyAndFixNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.App, (int)poolConfig.VerificationConfiguration.Connector.TotalNumberOfSentMessages);

            #endregion

            #region Save Relationships and Messages to Excel (and JSON, too)

            var jsonString = JsonSerializer.Serialize(relationshipAndMessagesList, new JsonSerializerOptions { WriteIndented = true });
            var jsonFilePath = Path.Combine(savePath, $"{RELATIONSHIPS_AND_MESSAGE_POOL_CONFIGS_FILE_NAME}.{workSheetName}.{JSON_FILE_EXT}");
            await File.WriteAllTextAsync(jsonFilePath, jsonString);

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
