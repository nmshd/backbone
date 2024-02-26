using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Messages.Domain.Entities;

public class RecipientInformation
{
    // ReSharper disable once UnusedMember.Local
    private RecipientInformation()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Address = null!;
        EncryptedKey = null!;
        RelationshipId = null!;
        MessageId = null!;
    }

    public RecipientInformation(IdentityAddress address, RelationshipId relationshipId, byte[] encryptedKey)
    {
        Address = address;
        RelationshipId = relationshipId;
        EncryptedKey = encryptedKey;
        MessageId = null!; // we just assign null to satisfy the compiler; it will be set by EF Core
    }

    public IdentityAddress Address { get; private set; }
    public byte[] EncryptedKey { get; }
    public DateTime? ReceivedAt { get; private set; }
    public DeviceId? ReceivedByDevice { get; private set; }
    public RelationshipId RelationshipId { get; }
    public MessageId MessageId { get; }

    public void FetchedMessage(DeviceId fetchedByDevice)
    {
        if (ReceivedAt.HasValue) return;

        ReceivedAt = SystemTime.UtcNow;
        ReceivedByDevice = fetchedByDevice;
    }

    internal void UpdateAddress(IdentityAddress newIdentityAddress)
    {
        Address = newIdentityAddress;
    }
}
