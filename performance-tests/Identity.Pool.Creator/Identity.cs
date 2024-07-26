using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.Datawallets.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.Messages.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;
using Backbone.Identity.Pool.Creator.PoolsFile;

namespace Backbone.Identity.Pool.Creator;
public class Identity
{
    public readonly UserCredentials UserCredentials;

    public uint UniqueOrderNumber;

    public List<Challenge>? Challenges;

    public List<string> DeviceIds { get; } = [];
    public string Address { private set; get; }

    public List<Identity> IdentitiesToSendMessagesTo { get; } = [];
    public List<Identity> IdentitiesToEstablishRelationshipsWith { get; } = [];
    public Dictionary<string, Identity> EstablishedRelationshipsById { get; internal set; } = [];
    public HashSet<(string messageId, Identity recipient)> SentMessagesIdRecipientPair { get; internal set; } = [];
    public uint RelationshipsCapacity { get; private set; }
    public uint ReceivedMessagesCapacity { get; set; }
    public uint SentMessagesCapacity { get; set; }

    public PoolEntry Pool { get; private set; }
    public string Nickname { get; }
    public string PoolType { get; }
    public uint GraphAlgorithmVisitCount { get; set; } = 0;
    public List<CreatedDatawalletModification> DatawalletModifications { get; internal set; } = [];
    public List<CreateRelationshipTemplateResponse> RelationshipTemplates { get; internal set; } = [];


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


        if (uniqueOrderNumber is not null)
        {
            UniqueOrderNumber = uniqueOrderNumber ?? 0;
        }
        else
        {
            pool.IdentityUniqueOrderNumbers.TryDequeue(out var nonNullableUniqueOrderNumber);
            UniqueOrderNumber = nonNullableUniqueOrderNumber;
        }
    }
    public void AddDevice(string deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    public bool HasAvailabilityForNewRelationships() => RelationshipsCapacity > 0;

    public bool AddIdentityToEstablishRelationshipsWith(Identity identity, bool skipCapacityCheck = false, bool isRecursiveCall = false)
    {
        if (!skipCapacityCheck && (!HasAvailabilityForNewRelationships() || !isRecursiveCall && !identity.HasAvailabilityForNewRelationships())) return false;
        if (IdentitiesToEstablishRelationshipsWith.Contains(identity)) return false;

        IdentitiesToEstablishRelationshipsWith.Add(identity);
        RelationshipsCapacity--;
        if (!isRecursiveCall) identity.AddIdentityToEstablishRelationshipsWith(this, skipCapacityCheck: skipCapacityCheck, isRecursiveCall: true);

        return true;
    }

    public void SendMessageTo(Identity recipient, bool skipCheck = false)
    {
        if (!IdentitiesToEstablishRelationshipsWith.Contains(recipient))
        {
            throw new Exception("Cannot send message to identity which does not have a relationship with this one.");
        }

        if (!skipCheck && (SentMessagesCapacity == 0 || !recipient.HasAvailabilityToReceiveNewMessages()))
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
