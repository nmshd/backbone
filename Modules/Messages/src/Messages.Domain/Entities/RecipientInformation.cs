using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Messages.Domain.Entities;

public class RecipientInformation : Entity
{
    // ReSharper disable once UnusedMember.Local
    private RecipientInformation()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Address = null!;
        RelationshipId = null!;
        EncryptedKey = null!;
        MessageId = null!;
    }

    public RecipientInformation(IdentityAddress address, RelationshipId relationshipId, byte[] encryptedKey)
    {
        Address = address;
        RelationshipId = relationshipId;
        EncryptedKey = encryptedKey;
        MessageId = null!; // we just assign null to satisfy the compiler; it will be set by EF Core
    }

    public int Id { get; }
    public RelationshipId RelationshipId { get; }
    public IdentityAddress Address { get; private set; }
    public byte[] EncryptedKey { get; }
    public DateTime? ReceivedAt { get; private set; }
    public DeviceId? ReceivedByDevice { get; private set; }
    public MessageId MessageId { get; }
    public bool IsRelationshipDecomposedByRecipient { get; private set; }
    public bool IsRelationshipDecomposedBySender { get; private set; }
    public bool IsRelationshipFullyDecomposed => IsRelationshipDecomposedByRecipient && IsRelationshipDecomposedBySender;

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

    internal void DecomposeSender()
    {
        if (IsRelationshipDecomposedBySender)
            throw new DomainException(DomainErrors.UnableToDecompose());

        IsRelationshipDecomposedBySender = true;
    }

    internal void DecomposeRecipient()
    {
        if (IsRelationshipDecomposedByRecipient)
            throw new DomainException(DomainErrors.UnableToDecompose());

        IsRelationshipDecomposedByRecipient = true;
    }
}
