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


                    Identity recipientConnectorIdentity = null;
                    foreach (var connectorIdentity in connectorIdentities)
                    {
                        var hasAppIdentityaConnectorIdentityRelationship = appIdentity.RelationshipAndMessages.Any(rm => rm.RecipientIdentityAddress == connectorIdentity.Address);
                        var hasConnectorIdentityanAppIdentityRelationship = connectorIdentity.RelationshipAndMessages.Any(rm => rm.RecipientIdentityAddress == appIdentity.Address);

                        if (hasAppIdentityaConnectorIdentityRelationship || hasConnectorIdentityanAppIdentityRelationship)
                        {
                            continue;
                        }

                        if (!connectorIdentity.HasAvailableRelationships)
                        {
                            continue;
                        }

                        recipientConnectorIdentity = connectorIdentity;
                        break;
                    }

                    if (recipientConnectorIdentity == null)
                    {
                        recipientConnectorIdentity = connectorIdentities.FirstOrDefault(c => c.HasAvailableRelationships);

                        if (recipientConnectorIdentity == null)
                        {
                            throw new InvalidOperationException(string.Format(RELATIONSHIP_NO_RECIPIENT_AVAILABLE, appIdentity.Address, appIdentity.PoolAlias));
                        }
                    }

                    var relationshipAndMessages = new RelationshipAndMessages(
                        SenderPool: appIdentity.PoolAlias,
                        SenderIdentityAddress: appIdentity.Address,
                        RecipientPool: recipientConnectorIdentity.PoolAlias,
                        RecipientIdentityAddress: recipientConnectorIdentity.Address);

                    appIdentity.RelationshipAndMessages.Add(relationshipAndMessages);

                    appIdentity.DecrementAvailableRelationships();

                    relationshipAndMessages.NumberOfSentMessages = appIdentity.HasAvailableRelationships
                        ? appIdentity.MessagesToSendPerRelationship
                        : appIdentity.MessagesToSendPerRelationship + appIdentity.ModuloSendMessages;


                    var reverseRelationshipAndMessages = new RelationshipAndMessages(
                        SenderPool: recipientConnectorIdentity.PoolAlias,
                        SenderIdentityAddress: recipientConnectorIdentity.Address,
                        RecipientPool: appIdentity.PoolAlias,
                        RecipientIdentityAddress: appIdentity.Address);

                    recipientConnectorIdentity.RelationshipAndMessages.Add(reverseRelationshipAndMessages);

                    recipientConnectorIdentity.DecrementAvailableRelationships();

                    var totalSentMessagesPerRelationship = recipientConnectorIdentity.NumberOfSentMessages / recipientConnectorIdentity.RelationshipAndMessages.Count;
                    var modulo = recipientConnectorIdentity.NumberOfSentMessages % recipientConnectorIdentity.RelationshipAndMessages.Count;

                    foreach (var relationshipAndMessage in recipientConnectorIdentity.RelationshipAndMessages)
                    {
                        relationshipAndMessage.NumberOfSentMessages = totalSentMessagesPerRelationship;
                    }

                    if (modulo == 0) continue;

                    var relationshipAndMessageWithModulo = recipientConnectorIdentity.RelationshipAndMessages.Last();
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

    internal static void VerifyNumberOfSentMessages(RelationshipAndMessages[] relationshipAndMessages, IdentityPoolType recipientIdentityPoolType, long expectedTotalNumberOfSentMessages)
    {
        var filteredRelationships = relationshipAndMessages.Where(rm => rm.RecipientIdentityPoolType == recipientIdentityPoolType).ToList();
        var actualTotalNumberOfSentMessages = filteredRelationships.Sum(rm => rm.NumberOfSentMessages);

        if (actualTotalNumberOfSentMessages == expectedTotalNumberOfSentMessages) return;

        var messageDifference = expectedTotalNumberOfSentMessages - actualTotalNumberOfSentMessages;

        if (messageDifference == 0) return;

        throw new InvalidOperationException(string.Format(VERIFICATION_TOTAL_NUMBER_OF_SENT_MESSAGES_FAILED, recipientIdentityPoolType, expectedTotalNumberOfSentMessages,
            actualTotalNumberOfSentMessages));
    }
}
