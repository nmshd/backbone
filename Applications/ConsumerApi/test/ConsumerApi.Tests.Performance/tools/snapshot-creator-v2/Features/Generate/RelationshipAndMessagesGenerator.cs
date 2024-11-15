using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Enums;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Generate;

public class RelationshipAndMessagesGenerator : IRelationshipAndMessagesGenerator
{
    public RelationshipAndMessages[] Generate(PerformanceTestConfiguration poolConfiguration)
    {
        if (!poolConfiguration.IsIdentityPoolConfigurationCreated) throw new InvalidOperationException(IDENTITY_POOL_CONFIGURATION_NOT_CREATED);

        var identityPools = poolConfiguration.IdentityPoolConfigurations;

        var appIdentities = identityPools
            .Where(ip => ip.Type == IdentityPoolType.App)
            .SelectMany(a => a.Identities)
            .ToList();
        var connectorIdentities = identityPools
            .Where(ip => ip.Type == IdentityPoolType.Connector)
            .SelectMany(c => c.Identities)
            .ToList();

        foreach (var appIdentity in appIdentities)
        {
            while (appIdentity.HasAvailableRelationships)
            {
                var availableConnectorIdentities = connectorIdentities
                    .Where(i => i.HasAvailableRelationships)
                    .ToList();


                IdentityConfiguration recipientConnectorIdentity = null;
                foreach (var connectorIdentity in availableConnectorIdentities)
                {
                    var hasRelationship =
                        // e.g. pool: a3 address: 1 has a forward-relationship with pool: c3 address: 1
                        appIdentity.RelationshipAndMessages
                            .Any(rm =>
                                rm.RecipientPoolAlias == connectorIdentity.PoolAlias &&
                                rm.RecipientIdentityAddress == connectorIdentity.Address) ||

                        // e.g. pool: c3 address: 1 has a reverse-relationship with pool: a3 address: 1
                        connectorIdentity.RelationshipAndMessages
                            .Any(rm =>
                                rm.RecipientPoolAlias == appIdentity.PoolAlias &&
                                rm.RecipientIdentityAddress == appIdentity.Address);

                    if (hasRelationship)
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
                    throw new InvalidOperationException($"No further recipient identity available to establish a relationship to sender identity Address: {appIdentity.Address} of {appIdentity.PoolAlias}");
                }

                var relationshipAndMessages = new RelationshipAndMessages(
                    SenderPoolAlias: appIdentity.PoolAlias,
                    SenderIdentityAddress: appIdentity.Address,
                    RecipientPoolAlias: recipientConnectorIdentity.PoolAlias,
                    RecipientIdentityAddress: recipientConnectorIdentity.Address);

                appIdentity.RelationshipAndMessages.Add(relationshipAndMessages);

                appIdentity.DecrementAvailableRelationships();

                relationshipAndMessages.NumberOfSentMessages = appIdentity.HasAvailableRelationships
                    ? appIdentity.MessagesToSendPerRelationship
                    : appIdentity.MessagesToSendPerRelationship + appIdentity.ModuloSendMessages;


                var reverseRelationshipAndMessages = new RelationshipAndMessages(
                    SenderPoolAlias: recipientConnectorIdentity.PoolAlias,
                    SenderIdentityAddress: recipientConnectorIdentity.Address,
                    RecipientPoolAlias: appIdentity.PoolAlias,
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


        var relationshipAndMessagesList =
            identityPools.SelectMany(ipr => ipr.Identities.SelectMany(i => i.RelationshipAndMessages)).ToArray();

        var relationShipCount = relationshipAndMessagesList.Length / 2; // Note: Div by 2 because a pair of relationships (forward/reverse) is equal to 1 relationship

        if (relationShipCount != poolConfiguration.VerificationConfiguration.TotalNumberOfRelationships)
        {
            throw new InvalidOperationException(string.Format(RELATIONSHIP_COUNT_MISMATCH, poolConfiguration.VerificationConfiguration.TotalNumberOfRelationships, relationShipCount));
        }

        VerifyNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.Connector, poolConfiguration.VerificationConfiguration.TotalAppSentMessages);
        VerifyNumberOfSentMessages(relationshipAndMessagesList, IdentityPoolType.App, poolConfiguration.VerificationConfiguration.TotalConnectorSentMessages);


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
