using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
internal record PoolsOffset
{
    public OffsetDirections? MessagesOffsetPendingTo { get; private set; }
    public OffsetDirections? RelationshipsOffsetPendingTo { get; private set; }

    public long MessagesOffset { get; private set; }
    public long RelationshipsOffset { get; private set; }

    /// <summary>
    /// We split the pools up into two groups: app and connector.
    /// The total of Messages & Relationships in each group should match.
    /// However, this is not trivial, and sometimes not even something we can achieve.
    /// Thus, this method can be used to determine if an extra pool must be created to accomodate
    /// the messages and relationships that are required but do not fit in the original pools.>
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


        return new PoolsOffset((appMessagesSum - connectorMessagesSum), appRelationshipsSum - connectorRelationshipsSum);
    }

    private PoolsOffset(long messages, long relationships)
    {
        MessagesOffset = long.Abs(messages);
        MessagesOffsetPendingTo = CalculateOffsetDirection(messages);

        RelationshipsOffset = long.Abs(relationships);
        RelationshipsOffsetPendingTo = CalculateOffsetDirection(relationships);
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
