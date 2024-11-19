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

                #region Attempt 1: Try find a connector identity for that app identity that has not established a relationship yet

                IdentityConfiguration? recipientConnectorIdentity = null;
                foreach (var connectorIdentity in availableConnectorIdentities)
                {
                    var hasRelationship = appIdentity.RelationshipAndMessages
                        .Any(rm =>
                            rm.RecipientPoolAlias == connectorIdentity.PoolAlias &&
                            rm.RecipientIdentityAddress == connectorIdentity.Address);

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

                #endregion

                if (recipientConnectorIdentity == null)
                {
                    #region Attempt 2: Try find another connector identity for that app identity and try to move an existing app identities relationship to another connector identity

                    var connectorIdentitiesThatCurrentAppIdentityHasNoRelationshipWith = connectorIdentities
                        .Where(c => !c.RelationshipAndMessages.Any(crm =>
                            crm.RecipientPoolAlias == appIdentity.PoolAlias &&
                            crm.RecipientIdentityAddress == appIdentity.Address))
                        .ToList();

                    if (connectorIdentitiesThatCurrentAppIdentityHasNoRelationshipWith.Count == 0)
                    {
                        throw new InvalidOperationException(
                            $"No further recipient identity available to establish a relationship to sender identity Address: {appIdentity.Address} of {appIdentity.PoolAlias}.");
                    }

                    foreach (var connectorIdentity in connectorIdentitiesThatCurrentAppIdentityHasNoRelationshipWith)
                    {
                        if (recipientConnectorIdentity != null)
                        {
                            break;
                        }

                        RelationshipAndMessages[] removeableRelationships = [.. connectorIdentity.RelationshipAndMessages];

                        foreach (var reverseRelationship in removeableRelationships)
                        {
                            #region Remove the relationship and retrieve appIdentityWithRelationship that has a relationship with current connector identity

                            connectorIdentity.RelationshipAndMessages.Remove(reverseRelationship);
                            connectorIdentity.IncrementAvailableRelationships();

                            var appIdentityWithRelationship = appIdentities.First(ai =>
                                ai.PoolAlias == reverseRelationship.RecipientPoolAlias &&
                                ai.Address == reverseRelationship.RecipientIdentityAddress);

                            var relationship = appIdentityWithRelationship.RelationshipAndMessages.First(rm =>
                                rm.RecipientPoolAlias == connectorIdentity.PoolAlias &&
                                rm.RecipientIdentityAddress == connectorIdentity.Address);

                            appIdentityWithRelationship.RelationshipAndMessages.Remove(relationship);
                            appIdentityWithRelationship.IncrementAvailableRelationships();

                            #endregion

                            #region Can appIdentityWithRelationship establish a relationship with any other connector identity?

                            var connectorIdentitiesExceptCurrentOne = connectorIdentities.Except([connectorIdentity]).ToList();

                            var availableConnectorIdentitiesForAppIdentityWithRelationship = connectorIdentitiesExceptCurrentOne
                                .Where(c => c.HasAvailableRelationships)
                                .Where(c => !c.RelationshipAndMessages.Any(crm =>
                                    crm.RecipientPoolAlias == appIdentityWithRelationship.PoolAlias &&
                                    crm.RecipientIdentityAddress == appIdentityWithRelationship.Address))
                                .ToList();

                            if (availableConnectorIdentitiesForAppIdentityWithRelationship.Count == 0)
                            {
                                appIdentityWithRelationship.RelationshipAndMessages.Add(relationship);
                                connectorIdentity.RelationshipAndMessages.Add(reverseRelationship);
                                continue;
                            }

                            #endregion

                            #region Create a new Relationship for appIdentityWithRelationship

                            var newRecipientConnectorIdentity = availableConnectorIdentitiesForAppIdentityWithRelationship.First();
                            CreateRelationship(appIdentityWithRelationship, newRecipientConnectorIdentity);

                            #endregion

                            recipientConnectorIdentity = connectorIdentity;
                            break;
                        }
                    }


                    if (recipientConnectorIdentity is null)
                    {
                        throw new InvalidOperationException(
                            $"No further recipient identity available to establish a relationship to sender identity Address: {appIdentity.Address} of {appIdentity.PoolAlias}.");
                    }

                    #endregion
                }

                CreateRelationship(appIdentity, recipientConnectorIdentity);
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

    private static void CreateRelationship(IdentityConfiguration senderIdentity, IdentityConfiguration recipientIdentity)
    {
        var relationshipAndMessages = new RelationshipAndMessages(
            SenderPoolAlias: senderIdentity.PoolAlias,
            SenderIdentityAddress: senderIdentity.Address,
            RecipientPoolAlias: recipientIdentity.PoolAlias,
            RecipientIdentityAddress: recipientIdentity.Address);

        senderIdentity.RelationshipAndMessages.Add(relationshipAndMessages);
        senderIdentity.DecrementAvailableRelationships();

        relationshipAndMessages.NumberOfSentMessages = senderIdentity.HasAvailableRelationships
            ? senderIdentity.MessagesToSendPerRelationship
            : senderIdentity.MessagesToSendPerRelationship + senderIdentity.ModuloSendMessages;


        var reverseRelationshipAndMessages = new RelationshipAndMessages(
            SenderPoolAlias: recipientIdentity.PoolAlias,
            SenderIdentityAddress: recipientIdentity.Address,
            RecipientPoolAlias: senderIdentity.PoolAlias,
            RecipientIdentityAddress: senderIdentity.Address);

        recipientIdentity.RelationshipAndMessages.Add(reverseRelationshipAndMessages);
        recipientIdentity.DecrementAvailableRelationships();

        var totalSentMessagesPerRelationship = recipientIdentity.NumberOfSentMessages / recipientIdentity.RelationshipAndMessages.Count;
        var modulo = recipientIdentity.NumberOfSentMessages % recipientIdentity.RelationshipAndMessages.Count;

        foreach (var relationshipAndMessage in recipientIdentity.RelationshipAndMessages)
        {
            relationshipAndMessage.NumberOfSentMessages = totalSentMessagesPerRelationship;
        }

        if (modulo == 0) return;

        var relationshipAndMessageWithModulo = recipientIdentity.RelationshipAndMessages.Last();
        relationshipAndMessageWithModulo.NumberOfSentMessages += modulo;
    }

    private static void VerifyNumberOfSentMessages(RelationshipAndMessages[] relationshipAndMessages, IdentityPoolType recipientIdentityPoolType, long expectedTotalNumberOfSentMessages)
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
