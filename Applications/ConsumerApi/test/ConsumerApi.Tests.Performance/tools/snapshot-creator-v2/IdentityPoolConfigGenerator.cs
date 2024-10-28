using System.Text.Json;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Readers;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Writers;
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


    public async Task<bool> VerifyPoolConfig(string excelFile, string workSheetName, string poolConfigJsonFile)
    {
        var poolConfigFromJson = await DeserializeFromJson(poolConfigJsonFile);
        var poolConfigFromExcel = await new PerformanceTestConfigurationExcelReader().Read(excelFile, workSheetName);
        var result = poolConfigFromJson.Equals(poolConfigFromExcel);
        return result;
    }

    public async Task<(bool Status, string Message)> GeneratePoolConfig(string excelFile, string workSheetName)
    {
        var filePath = Path.GetDirectoryName(excelFile);

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return (false, INVALID_FILE_PATH);
        }

        (bool Status, string Message) result;
        try
        {
            var poolConfigFromExcel = await new PerformanceTestConfigurationExcelReader().Read(excelFile, workSheetName);
            result = await new PoolConfigurationJsonWriter().Write(poolConfigFromExcel, workSheetName);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }


        return result;
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
                .ToList();

            #endregion

            #region Iterate over each App IdentityPool and App Identity and build a relationship to an available connector Identity

            var appIdentityPools = identityPools.Where(ip => ip.Type == IdentityPoolType.App).ToList();
            var connectorIdentityPools = identityPools.Where(ip => ip.Type == IdentityPoolType.Connector).ToList();

            foreach (var appIdentityPool in appIdentityPools)
            {
                foreach (var appIdentity in appIdentityPool.Identities)
                {
                    while (appIdentity.HasAvailableRelationships)
                    {
                        var connectorIdentities = connectorIdentityPools
                            .SelectMany(cip => cip.Identities)
                            .Where(i => i.HasAvailableRelationships)
                            .ToList();


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

                        var relationshipAndMessages = new RelationshipAndMessages(
                            SenderPool: appIdentity.PoolAlias,
                            SenderIdentityAddress: appIdentity.Address,
                            ReceiverPool: receiverConnectorIdentity.PoolAlias,
                            ReceiverIdentityAddress: receiverConnectorIdentity.Address,
                            ReceiverIdentityPoolType: receiverConnectorIdentity.IdentityPoolType);

                        appIdentity.RelationshipAndMessages.Add(relationshipAndMessages);

                        appIdentity.DecrementAvailableRelationships();

                        relationshipAndMessages.NumberOfSentMessages = appIdentity.HasAvailableRelationships
                            ? appIdentity.MessagesToSendPerRelationship
                            : appIdentity.MessagesToSendPerRelationship + appIdentity.ModuloSendMessages;


                        var reverseRelationshipAndMessages = new RelationshipAndMessages(
                            SenderPool: receiverConnectorIdentity.PoolAlias,
                            SenderIdentityAddress: receiverConnectorIdentity.Address,
                            ReceiverPool: appIdentity.PoolAlias,
                            ReceiverIdentityAddress: appIdentity.Address,
                            ReceiverIdentityPoolType: appIdentity.IdentityPoolType);

                        receiverConnectorIdentity.RelationshipAndMessages.Add(reverseRelationshipAndMessages);

                        receiverConnectorIdentity.DecrementAvailableRelationships();

                        var totalSentMessagesPerRelationship = receiverConnectorIdentity.NumberOfSentMessages / receiverConnectorIdentity.RelationshipAndMessages.Count;
                        var modulo = receiverConnectorIdentity.NumberOfSentMessages % receiverConnectorIdentity.RelationshipAndMessages.Count;

                        foreach (var relationshipAndMessage in receiverConnectorIdentity.RelationshipAndMessages)
                        {
                            relationshipAndMessage.NumberOfSentMessages = totalSentMessagesPerRelationship;
                        }

                        if (modulo == 0) continue;

                        var relationshipAndMessageWithModulo = receiverConnectorIdentity.RelationshipAndMessages.Last();
                        relationshipAndMessageWithModulo.NumberOfSentMessages += modulo;
                    }
                }
            }

            #endregion


            var relationshipAndMessagesList =
                identityPools.SelectMany(ipr => ipr.Identities.SelectMany(i => i.RelationshipAndMessages)).ToArray();

            var relationShipCount = relationshipAndMessagesList.Length / 2; // Note: Div by 2 because a pair of relationships (forward/reverse) is equal to 1 relationship

            if (relationShipCount != poolConfig.VerificationConfiguration.TotalNumberOfRelationships)
            {
                throw new InvalidOperationException(string.Format(RELATIONSHIP_COUNT_MISMATCH, poolConfig.VerificationConfiguration.TotalNumberOfRelationships, relationShipCount));
            }

            void VerifyNumberOfSentMessages(RelationshipAndMessages[] relationshipAndMessages, IdentityPoolType identityPoolType, long expectedTotalNumberOfSentMessages)
            {
                var filteredRelationships = relationshipAndMessages.Where(rm => rm.ReceiverIdentityPoolType == identityPoolType).ToList();
                var actualTotalNumberOfSentMessages = filteredRelationships.Sum(rm => rm.NumberOfSentMessages);

                if (actualTotalNumberOfSentMessages == expectedTotalNumberOfSentMessages) return;

                var messageDifference = expectedTotalNumberOfSentMessages - actualTotalNumberOfSentMessages;

                if (messageDifference == 0) return;

                throw new InvalidOperationException(string.Format(VERIFICATION_TOTAL_NUMBER_OF_SENT_MESSAGES_FAILED, identityPoolType, expectedTotalNumberOfSentMessages,
                    actualTotalNumberOfSentMessages));
            }

            VerifyNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.Connector, poolConfig.VerificationConfiguration.TotalAppSentMessages);
            VerifyNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.App, poolConfig.VerificationConfiguration.TotalConnectorSentMessages);


            await new ExcelMapper().SaveAsync(excelFilePath, relationshipAndMessagesList, workSheetName);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }

        return (true, excelFilePath);
    }
}
