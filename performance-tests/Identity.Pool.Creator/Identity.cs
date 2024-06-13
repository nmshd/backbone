using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Org.BouncyCastle.Asn1.Cms;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;

    public List<string> DeviceIds { get; } = [];
    public string Address { private set; get; }

    public List<Identity> IdentitiesToSendMessagesTo { get; } = [];
    public List<Identity> IdentitiesToEstablishRelationshipsWith { get; } = [];
    public uint RelationshipsCapacity { get; private set; }
    public uint ReceivedMessagesCapacity { get; set; }
    public uint SentMessagesCapacity { get; set; }

    public Identity(UserCredentials userCredentials, string address, string deviceId, PoolEntry pool, uint orderNumber)
    {
        Address = address;
        UserCredentials = userCredentials;
        DeviceIds.Add(deviceId);

        RelationshipsCapacity = pool.NumberOfRelationships;
        ReceivedMessagesCapacity = pool.NumberOfReceivedMessages;
        SentMessagesCapacity = pool.NumberOfSentMessages;

        Nickname = pool.Alias + orderNumber;
        PoolType = pool.Type;
    }



    public string Nickname { get; private set; }
    public string PoolType { get; }

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    public bool HasAvailabilityForNewRelationships() => RelationshipsCapacity > 0;

    public bool AddIdentityToEstablishRelationshipsWith(Identity identity, bool isRecursiveCall = false)
    {
        if (!HasAvailabilityForNewRelationships() || !isRecursiveCall && !identity.HasAvailabilityForNewRelationships()) return false;

        IdentitiesToEstablishRelationshipsWith.Add(identity);
        RelationshipsCapacity--;
        if (!isRecursiveCall) identity.AddIdentityToEstablishRelationshipsWith(this, true);

        return true;
    }

    public void SendMessageTo(Identity recipient)
    {
        if (!IdentitiesToEstablishRelationshipsWith.Contains(recipient))
        {
            throw new Exception("Cannot send message to identity which does not have a relationship with this one.");
        }

        if (SentMessagesCapacity == 0 || ReceivedMessagesCapacity == 0)
        {
            throw new Exception("There is no capacity to send this message.");
        }

        IdentitiesToSendMessagesTo.Add(recipient);
        SentMessagesCapacity--;
        recipient.ReceivedMessagesCapacity--;
    }

    public bool HasAvailabilityToReceiveNewMessages() => ReceivedMessagesCapacity > 0;
    public bool HasAvailabilityToSendNewMessages() => SentMessagesCapacity > 0;
}
