using Backbone.Identity.Pool.Creator.PoolsFile;
using Backbone.Identity.Pool.Creator.Tools;

namespace Backbone.Identity.Pool.Creator.Application.MessageDistributor;

public class MessageDistributorV2 : IMessageDistributor
{
    public void Distribute(IList<PoolEntry> pools)
    {
        var appIdentities = pools.Where(p => p.IsApp()).SelectMany(i => i.Identities).ToList();
        var connectorIdentities = pools.Where(p => p.IsConnector()).SelectMany(i => i.Identities).ToList();

        foreach (var identity in appIdentities)
        {
            // if this identity must send more messages than its peers can receive
            if (identity.SentMessagesCapacity > identity.IdentitiesToEstablishRelationshipsWith.Sum(i => i.ReceivedMessagesCapacity))
            {
                throw new Exception("Pool configuration is invalid or relationship allocation ran wrongfully.");
            }

            using var relatedIdentitiesIterator = identity.IdentitiesToEstablishRelationshipsWith.AsEnumerable().GetEnumerator();
            while (identity.HasAvailabilityToSendNewMessages())
            {
                var peerIdentity = relatedIdentitiesIterator.NextOrFirst();
                
                if (peerIdentity.HasAvailabilityToReceiveNewMessages())
                    identity.SendMessageTo(peerIdentity);
            }
        }
    }
}
