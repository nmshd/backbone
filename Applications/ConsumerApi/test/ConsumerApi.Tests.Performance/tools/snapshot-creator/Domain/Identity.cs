using Backbone.ConsumerApi.Sdk.Authentication;
using Backbone.ConsumerApi.Sdk.Endpoints.Challenges.Types;
using Backbone.ConsumerApi.Sdk.Endpoints.RelationshipTemplates.Types.Responses;
using Backbone.ConsumerApi.Sdk.Endpoints.SyncRuns.Types;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.PoolsFile;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.Domain;

public class Identity
{
    public readonly UserCredentials UserCredentials;

    public uint UniqueOrderNumber;

    public List<Challenge>? Challenges;

    public List<string> DeviceIds { get; } = [];
    public string Address { get; private set; }

    public List<Identity> IdentitiesToSendMessagesTo { get; } = [];
    public List<Identity> IdentitiesToEstablishRelationshipsWith { get; } = [];
    public Dictionary<string, Identity> EstablishedRelationshipsById { get; private set; } = [];
    public HashSet<(string messageId, Identity recipient)> SentMessagesIdRecipientPair { get; private set; } = [];
    public uint RelationshipsCapacity { get; private set; }
    public uint ReceivedMessagesCapacity { get; private set; }
    public uint SentMessagesCapacity { get; private set; }

    public PoolEntry Pool { get; private set; }
    public string Nickname { get; }
    public string PoolType { get; }
    public List<CreatedDatawalletModification> DatawalletModifications { get; private set; } = [];
    public List<CreateRelationshipTemplateResponse> RelationshipTemplates { get; private set; } = [];


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
            UniqueOrderNumber = (uint)uniqueOrderNumber;
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

    private void ReceiveMessageFrom(Identity _)
    {
        ReceivedMessagesCapacity--;
    }

    public bool HasAvailabilityToReceiveNewMessages() => ReceivedMessagesCapacity > 0;
    public bool HasAvailabilityToSendNewMessages() => SentMessagesCapacity > 0;

    public override string ToString() => Nickname;

    public void SetDatawalletModifications(List<CreatedDatawalletModification> resultDatawalletModifications)
    {
        DatawalletModifications = resultDatawalletModifications;
    }
}
