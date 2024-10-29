using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Interfaces;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Generator;

public class RelationshipAndMessagesGenerator : IRelationshipAndMessagesGenerator
{
    public RelationshipAndMessages[] Generate(PerformanceTestConfiguration performanceTestConfiguration)
    {
        var identityPools = performanceTestConfiguration.IdentityPoolConfigs
            .Select(poolConfigIdentityPoolConfig => new IdentityPool(poolConfigIdentityPoolConfig))
            .ToList();

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
                        ReceiverIdentityAddress: receiverConnectorIdentity.Address);

                    appIdentity.RelationshipAndMessages.Add(relationshipAndMessages);

                    appIdentity.DecrementAvailableRelationships();

                    relationshipAndMessages.NumberOfSentMessages = appIdentity.HasAvailableRelationships
                        ? appIdentity.MessagesToSendPerRelationship
                        : appIdentity.MessagesToSendPerRelationship + appIdentity.ModuloSendMessages;


                    var reverseRelationshipAndMessages = new RelationshipAndMessages(
                        SenderPool: receiverConnectorIdentity.PoolAlias,
                        SenderIdentityAddress: receiverConnectorIdentity.Address,
                        ReceiverPool: appIdentity.PoolAlias,
                        ReceiverIdentityAddress: appIdentity.Address);

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


        var relationshipAndMessagesList =
            identityPools.SelectMany(ipr => ipr.Identities.SelectMany(i => i.RelationshipAndMessages)).ToArray();

        var relationShipCount = relationshipAndMessagesList.Length / 2; // Note: Div by 2 because a pair of relationships (forward/reverse) is equal to 1 relationship

        if (relationShipCount != performanceTestConfiguration.VerificationConfiguration.TotalNumberOfRelationships)
        {
            throw new InvalidOperationException(string.Format(RELATIONSHIP_COUNT_MISMATCH, performanceTestConfiguration.VerificationConfiguration.TotalNumberOfRelationships, relationShipCount));
        }

        VerifyNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.Connector, performanceTestConfiguration.VerificationConfiguration.TotalAppSentMessages);
        VerifyNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.App, performanceTestConfiguration.VerificationConfiguration.TotalConnectorSentMessages);


        return relationshipAndMessagesList;
    }

    internal static void VerifyNumberOfSentMessages(RelationshipAndMessages[] relationshipAndMessages, IdentityPoolType identityPoolType, long expectedTotalNumberOfSentMessages)
    {
        var filteredRelationships = relationshipAndMessages.Where(rm => rm.ReceiverIdentityPoolType == identityPoolType).ToList();
        var actualTotalNumberOfSentMessages = filteredRelationships.Sum(rm => rm.NumberOfSentMessages);

        if (actualTotalNumberOfSentMessages == expectedTotalNumberOfSentMessages) return;

        var messageDifference = expectedTotalNumberOfSentMessages - actualTotalNumberOfSentMessages;

        if (messageDifference == 0) return;

        throw new InvalidOperationException(string.Format(VERIFICATION_TOTAL_NUMBER_OF_SENT_MESSAGES_FAILED, identityPoolType, expectedTotalNumberOfSentMessages,
            actualTotalNumberOfSentMessages));
    }
}
