using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.Checkers;
public static class CheckSolutionExtensionMethods
{
    public static (bool success, List<string> messages) CheckSolution(this IList<PoolEntry> pools)
    {
        var messages = new List<string>();

        foreach (var poolEntry in pools)
        {
            foreach (var identity in poolEntry.Identities)
            {
                var identityRelationshipsCount = identity.IdentitiesToEstablishRelationshipsWith.Count;
                var identityDistinctRelationshipsCount = identity.IdentitiesToEstablishRelationshipsWith.Distinct().Count();
                if (identityDistinctRelationshipsCount != identityRelationshipsCount)
                    messages.Add($"Relationships: found {Math.Abs(identityDistinctRelationshipsCount - identityRelationshipsCount)} repeated relationships for Identity {identity}.");
            }
        }

        foreach (var poolEntry in pools)
        {
            foreach (var identity in poolEntry.Identities)
            {
                var identitySentMessagesCount = identity.IdentitiesToSendMessagesTo.Count;
                var identityExpectedSentMessagesCount = identity.Pool.NumberOfSentMessages;
                if (identitySentMessagesCount > identityExpectedSentMessagesCount)
                    messages.Add($"Messages: Identity {identity} sent more {Math.Abs(identitySentMessagesCount - identityExpectedSentMessagesCount)} messages than allowed.");
            }
        }

        var expectedNumberOfEstablishedRelationships = pools.ExpectedNumberOfRelationships();
        var numberOfEstablishedRelationships = pools.NumberOfEstablishedRelationships();

        var expectedNumberOfSentMessages = pools.ExpectedNumberOfSentMessages();
        var numberOfSentMessages = pools.NumberOfSentMessages();

        if (expectedNumberOfSentMessages != numberOfSentMessages)
            messages.Add($"Relationships: expected {expectedNumberOfEstablishedRelationships} but found {numberOfEstablishedRelationships} instead (difference {Math.Abs(numberOfEstablishedRelationships - expectedNumberOfEstablishedRelationships)}).");

        if (expectedNumberOfSentMessages != numberOfSentMessages)
            messages.Add($"Messages: expected {expectedNumberOfSentMessages} but found {numberOfSentMessages} instead (difference of {Math.Abs(numberOfSentMessages - expectedNumberOfSentMessages)}).");

        return (messages.Count == 0 , messages);
    }
}
