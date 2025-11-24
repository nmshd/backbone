namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;

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
    public static PoolsOffset CalculatePoolOffsets(IList<PoolEntry> pools)
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

    public static List<PoolEntry> CreateOffsetPools(List<PoolEntry> pools, PoolsOffset? offset = null)
    {
        offset ??= CalculatePoolOffsets(pools);

        if (offset.RelationshipsOffset != 0)
        {
            var avgCeiling = Convert.ToUInt32(Math.Ceiling(pools.Where(p => p.NumberOfRelationships > 0).Average(p => p.NumberOfRelationships)));
            var otherPoolsRelationshipsAverage = avgCeiling % 2 == 0 ? avgCeiling : avgCeiling - 1;
            if (offset.RelationshipsOffset < otherPoolsRelationshipsAverage / 2)
            {
                otherPoolsRelationshipsAverage = Convert.ToUInt32(offset.RelationshipsOffset / 10);
            }

            pools.Add(new PoolEntry
            {
                Name = $"{(offset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "Connector" : "App")} Offset Pool for Relationships",
                NumberOfDevices = 1,
                Amount = Convert.ToUInt32(offset.RelationshipsOffset / otherPoolsRelationshipsAverage),
                Alias = offset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "c0r" : "a0r",
                NumberOfRelationships = otherPoolsRelationshipsAverage,
                Type = offset.RelationshipsOffsetPendingTo == OffsetDirections.App ? "connector" : "app"
            });
        }

        if (offset.MessagesOffset != 0)
        {
            var messagesOffsetPool1 = new PoolEntry
            {
                Name = $"{(offset.MessagesOffsetPendingTo == OffsetDirections.App ? "Connector" : "App")} Offset Pool for Messages",
                NumberOfDevices = 1,
                Amount = 1,
                Alias = offset.MessagesOffsetPendingTo == OffsetDirections.App ? "c0m" : "a0m",
                NumberOfRelationships = 1,
                TotalNumberOfMessages = Convert.ToUInt32(offset.MessagesOffset),
                Type = offset.MessagesOffsetPendingTo == OffsetDirections.App ? "connector" : "app"
            };

            // this pool is created simply to balance the 1 relationship created by the Pool above.
            var messagesOffsetPool2 = new PoolEntry
            {
                Name = $"{(offset.MessagesOffsetPendingTo == OffsetDirections.App ? "App" : "Connector")} Compensation Offset Pool for Messages",
                NumberOfDevices = 1,
                Amount = 1,
                Alias = offset.MessagesOffsetPendingTo == OffsetDirections.App ? "a0mc" : "c0mc",
                NumberOfRelationships = 1,
                Type = offset.MessagesOffsetPendingTo == OffsetDirections.App ? "app" : "connector"
            };

            pools.Add(messagesOffsetPool1);
            pools.Add(messagesOffsetPool2);
        }

        return pools;
    }
}

/// <summary>
/// The kind that has the most of some entity, e.g. relationships
/// </summary>
internal enum OffsetDirections { App, Connector }
