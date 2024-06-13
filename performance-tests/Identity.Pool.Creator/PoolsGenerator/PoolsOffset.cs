using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
internal record PoolsOffset
{
    public OffsetDirections? MessagesOffsetPendingTo { get; private set; }
    public OffsetDirections? RelationshipsOffsetPendingTo { get; private set; }
    public OffsetDirections? SentMessagesOffsetPendingTo { get; private set; }
    public OffsetDirections? ReceivedMessagesOffsetPendingTo { get; private set; }

    public long MessagesOffset { get; private set; }
    public long RelationshipsOffset { get; private set; }
    public long SentMessagesOffset { get; private set; }
    public long ReceivedMessagesOffset { get; private set; }

    /// <summary>
    /// We split the pools up into two groups: app and connector.
    /// The total of Messages & Relationships in each group should match.
    ///
    /// Moreover, the number of messages sent by the app pools
    /// should match the number of messages received by the connector pools and vice-versa.
    ///
    /// However, this is not trivial, and sometimes not even something we can achieve.
    /// Thus, this method can be used to determine if extra pools must be created to accomodate
    /// the messages and relationships that are required.
    /// </summary>
    /// <param name="pools"></param>
    public static PoolsOffset CalculatePoolOffsets(PoolEntry[] pools)
    {
        var appPools = pools.Where(p => p.IsApp()).ToList();
        var connectorPools = pools.Where(p => p.IsConnector()).ToList();

        var appMessagesSum = appPools.Sum(p => p.TotalNumberOfMessages * p.Amount);
        var appRelationshipsSum = appPools.Sum(p => p.NumberOfRelationships * p.Amount);

        var connectorMessagesSum = connectorPools.Sum(p => p.TotalNumberOfMessages * p.Amount);
        var connectorRelationshipsSum = connectorPools.Sum(p => p.NumberOfRelationships * p.Amount);

        var messagesSentByConnectorSum = connectorPools.Sum(p => p.NumberOfSentMessages);
        var messagesSentByAppSum = appPools.Sum(p => p.NumberOfSentMessages);
        var messagesReceivedByConnectorSum = connectorPools.Sum(p => p.NumberOfReceivedMessages);
        var messagesReceivedByAppSum = appPools.Sum(p => p.NumberOfReceivedMessages);

        return new PoolsOffset(appMessagesSum - connectorMessagesSum,
            appRelationshipsSum - connectorRelationshipsSum,
            messagesSentByAppSum - messagesSentByConnectorSum,
            messagesReceivedByAppSum - messagesReceivedByConnectorSum);
    }

    private PoolsOffset(long messages, long relationships, long sentMessages, long receivedMessages)
    {
        MessagesOffset = long.Abs(messages);
        MessagesOffsetPendingTo = CalculateOffsetDirection(messages);

        RelationshipsOffset = long.Abs(relationships);
        RelationshipsOffsetPendingTo = CalculateOffsetDirection(relationships);

        SentMessagesOffset = sentMessages;
        SentMessagesOffsetPendingTo = CalculateOffsetDirection(sentMessages);

        ReceivedMessagesOffset = receivedMessages;
        ReceivedMessagesOffsetPendingTo = CalculateOffsetDirection(receivedMessages);
    }

    private static OffsetDirections? CalculateOffsetDirection(long offset)
    {
        if (offset == 0) return null;
        return offset > 0 ? OffsetDirections.App : OffsetDirections.Connector;
    }
}

/// <summary>
/// The kind that has the most of some entity, e.g. relationships
/// </summary>
internal enum OffsetDirections { App, Connector }
