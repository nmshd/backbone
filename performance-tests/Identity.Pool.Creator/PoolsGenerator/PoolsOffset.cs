using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;
internal record PoolsOffset
{
    private PoolsOffset(long messages, long relationships)
    {
        MessagesOffset = messages;
        RelationshipsOffset = relationships;
    }

    private long MessagesOffset { get; set; }
    private long RelationshipsOffset { get; set; }

    /// <summary>
    /// We split the pools up into two groups: app and connector.
    /// The total of Messages & Relationships in each group should match.
    /// However, this is not trivial, and sometimes not even something we can achieve.
    /// Thus, this method can be used to determine if an extra pool must be created to accomodate
    /// the messages and relationships that are required but do not fit in the original pools.
    /// </summary>
    /// <param name="pools"></param>
    public static PoolsOffset CalculatePoolOffsets(PoolEntry[] pools)
    {
        var appPools = pools.Where(p => p.Type == "app").ToList();
        var connectorPools = pools.Where(p => p.Type == "connector").ToList();

        var appMessagesSum = appPools.Sum(p => p.NumberOfSentMessages * p.Amount);
        var appRelationshipsSum = appPools.Sum(p => p.NumberOfRelationships * p.Amount);

        var connectorMessagesSum = connectorPools.Sum(p => p.NumberOfSentMessages * p.Amount);
        var connectorRelationshipsSum = connectorPools.Sum(p => p.NumberOfRelationships * p.Amount);


        return new PoolsOffset((appMessagesSum - connectorMessagesSum), appRelationshipsSum - connectorRelationshipsSum);
    }
}
