using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;

public class RelationshipDistributorV3 : IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appPools = pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
        var connectorPools = pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();
        var appAndConnectorIdentities = pools.Where(p => p.IsApp() || p.IsConnector()).SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();

        var appPoolsIdentities = appPools.SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();
        var connectorPoolsIdentities = connectorPools.SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();

        foreach (var identity in appPoolsIdentities)
        {
            foreach (var connectorIdentity in connectorPoolsIdentities)
            {
                identity.AddIdentityToEstablishRelationshipsWith(connectorIdentity, skipCapacityCheck: true);
            }
        }
    }
}
