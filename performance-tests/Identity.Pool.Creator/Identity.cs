using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Identity.Pool.Creator.PoolsFile;
using Org.BouncyCastle.Asn1.Cms;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;
    
    /// <summary>
    /// Unique Order Number
    /// </summary>
    internal readonly uint Uon;

    public List<string> DeviceIds { get; } = [];
    public string Address { private set; get; }

    public List<Identity> IdentitiesToSendMessagesTo { get; } = [];
    public List<Identity> IdentitiesToEstablishRelationshipsWith { get; } = [];
    public uint RelationshipsCapacity { get; private set; }
    public uint ReceivedMessagesCapacity { get; set; }
    public uint SentMessagesCapacity { get; set; }

    public PoolEntry Pool { get; private set; }

    public Identity(UserCredentials userCredentials, string address, string deviceId, PoolEntry pool, uint orderNumber, uint? uniqueOrderNumber = null)
    {
        Address = address;
        UserCredentials = userCredentials;
        DeviceIds.Add(deviceId);

        RelationshipsCapacity = pool.NumberOfRelationships;
        ReceivedMessagesCapacity = pool.NumberOfReceivedMessages;
        SentMessagesCapacity = pool.NumberOfSentMessages;

        Nickname = pool.Alias + orderNumber;
        PoolType = pool.Type;
        Pool = pool;
        Uon = uniqueOrderNumber ?? 0;
    }

    public string Nickname { get; }
    public string PoolType { get; }
    public uint GraphAlgorithmVisitCount { get; set; } = 0;

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    public bool HasAvailabilityForNewRelationships() => RelationshipsCapacity > 0;

    public bool AddIdentityToEstablishRelationshipsWith(Identity identity, bool skipCapacityCheck = false, bool isRecursiveCall = false)
    {
        if (skipCapacityCheck && (!HasAvailabilityForNewRelationships() || !isRecursiveCall && !identity.HasAvailabilityForNewRelationships())) return false;
        if (IdentitiesToEstablishRelationshipsWith.Contains(identity)) return false;

        IdentitiesToEstablishRelationshipsWith.Add(identity);
        RelationshipsCapacity--;
        if (!isRecursiveCall) identity.AddIdentityToEstablishRelationshipsWith(this, skipCapacityCheck: skipCapacityCheck, isRecursiveCall: true);

        return true;
    }

    public void SendMessageTo(Identity recipient)
    {
        if (!IdentitiesToEstablishRelationshipsWith.Contains(recipient))
        {
            throw new Exception("Cannot send message to identity which does not have a relationship with this one.");
        }

        if (SentMessagesCapacity == 0 || !recipient.HasAvailabilityToReceiveNewMessages())
        {
            throw new Exception("There is no capacity to send this message.");
        }

        if (Nickname.First() == recipient.Nickname.First())
            throw new Exception($"Cannot send message from identity of type {Nickname.First()} to identity of the same type.");

        IdentitiesToSendMessagesTo.Add(recipient);
        recipient.ReceiveMessageFrom(this);
        SentMessagesCapacity--;
    }

    private void ReceiveMessageFrom(Identity identity)
    {
        ReceivedMessagesCapacity--;
    }

    public bool HasAvailabilityToReceiveNewMessages() => ReceivedMessagesCapacity > 0;
    public bool HasAvailabilityToSendNewMessages() => SentMessagesCapacity > 0;

    public override string ToString() => Nickname;
}
