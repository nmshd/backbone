using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;

public class RelationshipDistributorV2 : IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appPools = pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
        var connectorPools = pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();
        var appAndConnectorIdentities = pools.Where(p => p.IsApp() || p.IsConnector()).SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();

        var appPoolsIdentities = appPools.SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();
        var connectorPoolsIdentities = connectorPools.SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();

        var expectedRelationshipsCount = RelationshipDistributorTools.CheckRelationshipCounts(appPools, connectorPools);
        List<int> successfullyEstablishedRelationshipsCounts = [pools.HasMessagesOffsetPool() ? 1 : 0];

        while (expectedRelationshipsCount > successfullyEstablishedRelationshipsCounts.Last())
        {
            foreach (var identity in appAndConnectorIdentities.Where(i => i.HasAvailabilityForNewRelationships()))
            {
                successfullyEstablishedRelationshipsCounts.Add(DistributeRelationshipsV2InnerLoop(appPoolsIdentities, connectorPoolsIdentities, successfullyEstablishedRelationshipsCounts.Last(),
                    identity));
            }

            // break on convergence
            if (successfullyEstablishedRelationshipsCounts.Count > 80)
            {
                var length = successfullyEstablishedRelationshipsCounts.Count;
                if (successfullyEstablishedRelationshipsCounts[length - 1] == successfullyEstablishedRelationshipsCounts[length - 2] &&
                    successfullyEstablishedRelationshipsCounts[length - 3] == successfullyEstablishedRelationshipsCounts[length - 2])
                {
                    return;
                }
            }
        }
    }

    private static int DistributeRelationshipsV2InnerLoop(
        List<Identity> appPoolsIdentities,
        List<Identity> connectorPoolsIdentities,
        int successfullyEstablishedRelationshipsCount,
        Identity identity
        )
    {
        var oppositePoolIdentities = identity.PoolType == PoolTypes.CONNECTOR_TYPE ? appPoolsIdentities : connectorPoolsIdentities;
        var oppositePoolIdentitiesWithCapacityForNewRelationships = oppositePoolIdentities.Where(i => i.HasAvailabilityForNewRelationships()).ToList();
        Identity selectedIdentity;
        var index = 0;
        while (identity.RelationshipsCapacity > 0)
        {
            // We select the identity with the highest capacity for relationships and we fill it with an identity from an opposite pool.
            do
            {
                selectedIdentity = oppositePoolIdentitiesWithCapacityForNewRelationships[index++];
                if (index == oppositePoolIdentitiesWithCapacityForNewRelationships.Count)
                    return successfullyEstablishedRelationshipsCount;

            } while (identity.IdentitiesToEstablishRelationshipsWith.Contains(selectedIdentity));

            if (identity.AddIdentityToEstablishRelationshipsWith(selectedIdentity))
                successfullyEstablishedRelationshipsCount++;
        }

        return successfullyEstablishedRelationshipsCount;
    }
}
