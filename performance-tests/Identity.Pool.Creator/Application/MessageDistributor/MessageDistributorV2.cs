using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.Tools;

namespace Backbone.Identity.Pool.Creator.Application.MessageDistributor;

public class MessageDistributorV2 : IMessageDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appAndConnectorIdentities = pools.Where(p => p.IsApp() || p.IsConnector()).SelectMany(p => p.Identities).ToList();

        foreach (var identity in appAndConnectorIdentities)
        {
            using var relatedIdentitiesIterator = identity.IdentitiesToEstablishRelationshipsWith.AsEnumerable().GetEnumerator();
            var endReached = false;
            while (identity.HasAvailabilityToSendNewMessages() && !endReached)
            {
                var peerIdentity = relatedIdentitiesIterator.NextOrFirst();
                
                if (peerIdentity.HasAvailabilityToReceiveNewMessages())
                    identity.SendMessageTo(peerIdentity);

                if (peerIdentity == identity.IdentitiesToEstablishRelationshipsWith.Last())
                    endReached = true;
            }
        }
    }
}
