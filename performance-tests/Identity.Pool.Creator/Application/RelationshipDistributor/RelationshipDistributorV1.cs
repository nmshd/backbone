using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator.Application.RelationshipDistributor;

public class RelationshipDistributorV1 : IRelationshipDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appPools = pools.Where(p => p.IsApp() && p.NumberOfRelationships > 0).ToList();
        var connectorPools = pools.Where(p => p.IsConnector() && p.NumberOfRelationships > 0).ToList();

        var appRelationshipsCount = RelationshipDistributorTools.CheckRelationshipCounts(appPools, connectorPools);

        var targetIdentities = connectorPools.SelectMany(p => p.Identities).ToArray();
        var targetIdentitiesIteratorIndex = 0;

        while (appPools.Sum(p => p.Identities.Sum(i => i.IdentitiesToEstablishRelationshipsWith.Count)) < appRelationshipsCount)
        {
            foreach (var poolEntry in appPools)
            {
                foreach (var identity in poolEntry.Identities.Where(i => i.HasAvailabilityForNewRelationships()))
                {
                    Identity candidateIdentityForRelationship;
                    var counter = 0;
                    do
                    {
                        if (counter++ > targetIdentities.Length)
                        {
                            throw new Exception("The current algorithm cannot handle this input because the attribution of relationships is not trivial.");
                        }
                        candidateIdentityForRelationship = GetCandidateIdentityForRelationship(ref targetIdentities, ref targetIdentitiesIteratorIndex);

                    }
                    while (identity.IdentitiesToEstablishRelationshipsWith.Contains(candidateIdentityForRelationship));

                    identity.AddIdentityToEstablishRelationshipsWith(candidateIdentityForRelationship);
                }
            }
        }
    }

    private static Identity GetCandidateIdentityForRelationship(ref Identity[] targetIdentities, ref int targetIdentitiesIteratorIndex)
    {
        var candidateIdentityForRelationship = targetIdentities[targetIdentitiesIteratorIndex];
        if (!candidateIdentityForRelationship.HasAvailabilityForNewRelationships())
        {
            // this identity has been exhausted and can be removed from the iteration list.
            targetIdentities = targetIdentities.Except([candidateIdentityForRelationship]).ToArray();
        }
        else
        {
            targetIdentitiesIteratorIndex++;
        }

        if (targetIdentitiesIteratorIndex == targetIdentities.Length - 1)
        {
            // removed the last item and fell out of the indexes. reset
            targetIdentitiesIteratorIndex = 0;
        }

        return candidateIdentityForRelationship;
    }
}
