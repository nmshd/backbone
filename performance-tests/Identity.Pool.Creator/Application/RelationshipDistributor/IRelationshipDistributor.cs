using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;

public interface IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools);
}

public class RelationshipDistributorTools
{
    protected internal static long CheckRelationshipCounts(IList<PoolEntry> appPools, IList<PoolEntry> connectorPools)
    {
        var appRelationshipsCount = appPools.Sum(p => p.NumberOfRelationships * p.Amount);
        var connectorRelationshipsCount = connectorPools.Sum(p => p.NumberOfRelationships * p.Amount);

        if (appRelationshipsCount != connectorRelationshipsCount)
            throw new Exception(
                "The number of relationships in the app pools does not match the number of relationships in the connector pools, despite there being offset pools. There is an implementation error.");
        return appRelationshipsCount;
    }

    protected internal static void EstablishMessagesOffsetPoolsRelationships(IList<PoolEntry> pools)
    {
        var messagesOffsetPools = pools.Where(p => p.Alias.StartsWith("a0m") || p.Alias.StartsWith("c0m")).ToList();
        if (messagesOffsetPools.Count != 2) return;

        var (p1, p2) = (messagesOffsetPools[0], messagesOffsetPools[1]);
        p1.Identities.Single().AddIdentityToEstablishRelationshipsWith(p2.Identities.Single());
    }
}
