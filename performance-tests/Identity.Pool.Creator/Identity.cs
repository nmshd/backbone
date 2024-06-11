using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;

    public List<string> DeviceIds { get; } = [];
    public string Address { private set; get; }

    public bool HasBeenUsedAsMessageRecipient { get; private set; }
    //public uint RelationshipsTarget { get; private set; } = 0;

    public List<Identity> IdentitiesToEstablishRelationshipsWith { get; }
    public uint RelationshipsAvailable { get; private set; }
    public Identity(UserCredentials userCredentials, string address, string deviceId, PoolEntry pool, uint orderNumber)
    {
        Address = address;
        UserCredentials = userCredentials;
        DeviceIds.Add(deviceId);
        HasBeenUsedAsMessageRecipient = false;

        RelationshipsAvailable = pool.NumberOfRelationships;
        IdentitiesToEstablishRelationshipsWith = [];
        Nickname = pool.Alias + orderNumber;
        PoolType = pool.Type;
    }

    public string Nickname { get; private set; }
    public string PoolType { get; }

    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    public void UseAsMessageRecipient()
    {
        HasBeenUsedAsMessageRecipient = true;
    }

    public bool HasAvailabilityForNewRelationships() => RelationshipsAvailable > 0;

    public bool AddIdentityToEstablishRelationshipsWith(Identity identity, bool isRecursiveCall = false)
    {
        if (!HasAvailabilityForNewRelationships() || !identity.HasAvailabilityForNewRelationships()) return false;

        IdentitiesToEstablishRelationshipsWith.Add(identity);
        RelationshipsAvailable--;
        if (!isRecursiveCall) identity.AddIdentityToEstablishRelationshipsWith(this, true);

        return true;
    }
}
