using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Messages.Domain.Ids;

namespace Messages.Domain.Entities;

public class RecipientInformation
{
#pragma warning disable CS8618
    private RecipientInformation() { }
#pragma warning restore CS8618

#pragma warning disable CS8618
    public RecipientInformation(IdentityAddress address, RelationshipId relationshipId, byte[] encryptedKey)
#pragma warning restore CS8618
    {
        Address = address;
        RelationshipId = relationshipId;
        EncryptedKey = encryptedKey;
    }

    public IdentityAddress Address { get; }
    public byte[] EncryptedKey { get; }
    public DateTime? ReceivedAt { get; private set; }
    public DeviceId? ReceivedByDevice { get; private set; }
    public RelationshipId RelationshipId { get; }
    public MessageId MessageId { get; }

    public void ReceivedMessage(DeviceId receivedByDevice)
    {
        ReceivedAt = SystemTime.UtcNow;
        ReceivedByDevice = receivedByDevice;
    }
}
