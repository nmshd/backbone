using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public class RelationshipAndMessagesGenerator : IRelationshipAndMessagesGenerator
{
    public RelationshipAndMessages[] Generate(PerformanceTestConfiguration performanceTestConfiguration)
    {
        if (!performanceTestConfiguration.IsIdentityPoolConfigurationCreated) throw new InvalidOperationException(IDENTITY_POOL_CONFIGURATION_NOT_CREATED);

        var identityPools = performanceTestConfiguration.IdentityPoolConfigurations;

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


                    IdentityConfiguration recipientConnectorIdentityConfiguration = null;
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

                        recipientConnectorIdentityConfiguration = connectorIdentity;
                        break;
                    }

                    if (recipientConnectorIdentityConfiguration == null)
                    {
                        recipientConnectorIdentityConfiguration = connectorIdentities.FirstOrDefault(c => c.HasAvailableRelationships);

                        if (recipientConnectorIdentityConfiguration == null)
                        {
                            throw new InvalidOperationException(string.Format(RELATIONSHIP_NO_RECIPIENT_AVAILABLE, appIdentity.Address, appIdentity.PoolAlias));
                        }
                    }

                    var relationshipAndMessages = new RelationshipAndMessages(
                        SenderPool: appIdentity.PoolAlias,
                        SenderIdentityAddress: appIdentity.Address,
                        RecipientPool: recipientConnectorIdentityConfiguration.PoolAlias,
                        RecipientIdentityAddress: recipientConnectorIdentityConfiguration.Address);

                    appIdentity.RelationshipAndMessages.Add(relationshipAndMessages);

                    appIdentity.DecrementAvailableRelationships();

                    relationshipAndMessages.NumberOfSentMessages = appIdentity.HasAvailableRelationships
                        ? appIdentity.MessagesToSendPerRelationship
                        : appIdentity.MessagesToSendPerRelationship + appIdentity.ModuloSendMessages;


                    var reverseRelationshipAndMessages = new RelationshipAndMessages(
                        SenderPool: recipientConnectorIdentityConfiguration.PoolAlias,
                        SenderIdentityAddress: recipientConnectorIdentityConfiguration.Address,
                        RecipientPool: appIdentity.PoolAlias,
                        RecipientIdentityAddress: appIdentity.Address);

                    recipientConnectorIdentityConfiguration.RelationshipAndMessages.Add(reverseRelationshipAndMessages);

                    recipientConnectorIdentityConfiguration.DecrementAvailableRelationships();

                    var totalSentMessagesPerRelationship = recipientConnectorIdentityConfiguration.NumberOfSentMessages / recipientConnectorIdentityConfiguration.RelationshipAndMessages.Count;
                    var modulo = recipientConnectorIdentityConfiguration.NumberOfSentMessages % recipientConnectorIdentityConfiguration.RelationshipAndMessages.Count;

                    foreach (var relationshipAndMessage in recipientConnectorIdentityConfiguration.RelationshipAndMessages)
                    {
                        relationshipAndMessage.NumberOfSentMessages = totalSentMessagesPerRelationship;
                    }

                    if (modulo == 0) continue;

                    var relationshipAndMessageWithModulo = recipientConnectorIdentityConfiguration.RelationshipAndMessages.Last();
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
