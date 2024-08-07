using Backbone.PerformanceSnapshotCreator.PoolsFile;

namespace Backbone.PerformanceSnapshotCreator.Application.RelationshipDistributor;

/// <summary>
/// Assigns relationships based on a special heuristic called PWA (Pool Weight Affinity)
/// Trying to match pools with similar weights first.
/// Starts with heavy ones and proceeds to lighter ones.
/// </summary>
public class RelationshipDistributorV4 : IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appPools = pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
        var connectorPools = pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();
        var appAndConnectorIdentities = pools.Where(p => p.IsApp() || p.IsConnector()).SelectMany(p => p.Identities).OrderByDescending(i => i.RelationshipsCapacity).ToList();

        var appPoolsIdentities = appPools.SelectMany(p => p.Identities).OrderByDescending(i => i.Pool.Alias).ThenBy(i => i.RelationshipsCapacity).ToList();
        var connectorPoolsIdentities = connectorPools.SelectMany(p => p.Identities).OrderByDescending(i => i.Pool.Alias).ThenBy(i => i.RelationshipsCapacity).ToList();

        var expectedRelationshipsCount = CheckRelationshipCounts(appPools, connectorPools);
        List<int> successfullyEstablishedRelationshipsCounts = [pools.HasMessagesOffsetPool() ? 1 : 0];

        while (expectedRelationshipsCount > successfullyEstablishedRelationshipsCounts.Last())
        {
            foreach (var identity in appAndConnectorIdentities.Where(i => i.HasAvailabilityForNewRelationships()))
            {
                successfullyEstablishedRelationshipsCounts.Add(DistributeRelationshipsV2InnerLoop(appPoolsIdentities, connectorPoolsIdentities, successfullyEstablishedRelationshipsCounts.Last(),
                    identity));
            }

            // break on convergence
            if (successfullyEstablishedRelationshipsCounts.Count <= 80) continue;

            var length = successfullyEstablishedRelationshipsCounts.Count;
            if (successfullyEstablishedRelationshipsCounts[length - 1] == successfullyEstablishedRelationshipsCounts[length - 2] &&
                successfullyEstablishedRelationshipsCounts[length - 3] == successfullyEstablishedRelationshipsCounts[length - 2])
            {
                return;
            }
        }
    }

    private static int DistributeRelationshipsV2InnerLoop(
        List<Domain.Identity> appPoolsIdentities,
        List<Domain.Identity> connectorPoolsIdentities,
        int successfullyEstablishedRelationshipsCount,
        Domain.Identity identity
        )
    {
        var oppositePoolIdentitiesWithCapacityForFurtherRelationships = (identity.PoolType == PoolTypes.CONNECTOR_TYPE
                ? appPoolsIdentities
                : connectorPoolsIdentities
                ).Where(i => i.HasAvailabilityForNewRelationships()).OrderBy(i => i.IdentitiesToEstablishRelationshipsWith.Count)
            .ToList();

        var index = 0;
        while (identity.RelationshipsCapacity > 0)
        {
            Domain.Identity selectedIdentity;
            do
            {
                if (index == oppositePoolIdentitiesWithCapacityForFurtherRelationships.Count)
                    return successfullyEstablishedRelationshipsCount;
                selectedIdentity = oppositePoolIdentitiesWithCapacityForFurtherRelationships[index++];
            } while (identity.IdentitiesToEstablishRelationshipsWith.Contains(selectedIdentity));

            if (identity.AddIdentityToEstablishRelationshipsWith(selectedIdentity))
                successfullyEstablishedRelationshipsCount++;
        }

        return successfullyEstablishedRelationshipsCount;
    }

    protected internal static long CheckRelationshipCounts(IList<PoolEntry> appPools, IList<PoolEntry> connectorPools)
    {
        var appRelationshipsCount = appPools.Sum(p => p.NumberOfRelationships * p.Amount);
        var connectorRelationshipsCount = connectorPools.Sum(p => p.NumberOfRelationships * p.Amount);

        if (appRelationshipsCount != connectorRelationshipsCount)
            throw new Exception(
                "The number of relationships in the app pools does not match the number of relationships in the connector pools, despite there being offset pools. There is an implementation error.");
        return appRelationshipsCount;
    }
}
