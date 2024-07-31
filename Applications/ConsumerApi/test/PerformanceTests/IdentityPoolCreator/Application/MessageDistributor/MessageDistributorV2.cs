using Backbone.PerformanceSnapshotCreator.PoolsFile;
using Backbone.PerformanceSnapshotCreator.Tools;

namespace Backbone.PerformanceSnapshotCreator.Application.MessageDistributor;

public class MessageDistributorV2 : IMessageDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appAndConnectorIdentities = pools.Where(p => p.IsApp() || p.IsConnector()).SelectMany(p => p.Identities).ToList();

        uint sentMessages;

        do
        {
            sentMessages = 0;
            foreach (var identity in appAndConnectorIdentities)
            {
                using var relatedIdentitiesIterator = identity.IdentitiesToEstablishRelationshipsWith.AsEnumerable().GetEnumerator();
                var endReached = false;
                while (identity.HasAvailabilityToSendNewMessages() && !endReached)
                {
                    var peerIdentity = relatedIdentitiesIterator.NextOrFirst();

                    if (peerIdentity.HasAvailabilityToReceiveNewMessages())
                    {
                        identity.SendMessageTo(peerIdentity);
                        sentMessages++;
                    }

                    if (peerIdentity == identity.IdentitiesToEstablishRelationshipsWith.Last())
                        endReached = true;
                }
            }
        } while (sentMessages > 0);
    }
}
