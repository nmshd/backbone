using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Message : Entity
{
    // ReSharper disable once UnusedMember.Local
    private Message()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Body = null!;
        Attachments = null!;
        Recipients = null!;
    }

    public Message(IdentityAddress createdBy, DeviceId createdByDevice, byte[] body, IEnumerable<Attachment> attachments, IEnumerable<RecipientInformation> recipients)
    {
        Id = MessageId.New();
        CreatedAt = SystemTime.UtcNow;
        Recipients = recipients.ToList();

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        Body = new MessageBody(Id, body);
        Attachments = attachments.ToList();

        RaiseDomainEvent(new MessageCreatedDomainEvent(this));
    }

    public MessageId Id { get; }

    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; private set; }
    public DeviceId CreatedByDevice { get; }

    public virtual MessageBody Body { get; }

    public virtual IReadOnlyCollection<Attachment> Attachments { get; }
    public virtual IReadOnlyCollection<RecipientInformation> Recipients { get; }

    private bool CanAnonymizeSender => Recipients.All(r => r.IsRelationshipFullyDecomposed);

    public void AnonymizeParticipant(IdentityAddress participantAddress, IdentityAddress anonymizedAddress)
    {
        if (CreatedBy == participantAddress)
            AnonymizeSender(anonymizedAddress);

        AnonymizeRecipient(participantAddress, anonymizedAddress);

        if (IsOrphaned(this, anonymizedAddress))
            RaiseDomainEvent(new MessageOrphanedDomainEvent(this));
    }

    public void DecomposeFor(IdentityAddress decomposerAddress, IdentityAddress peerAddress, IdentityAddress anonymizedAddress)
    {
        var recipient = Recipients.FirstOrDefault(r => r.Address == decomposerAddress || r.Address == peerAddress) ?? throw new DomainException(DomainErrors.UnableToDecompose());

        SetDecompositionFlags(decomposerAddress, recipient);
        AnonymizeParticipants(recipient, anonymizedAddress);
    }

    private void SetDecompositionFlags(IdentityAddress decomposerAddress, RecipientInformation recipient)
    {
        if (decomposerAddress == recipient.Address)
            recipient.DecomposeRecipient();

        if (decomposerAddress == CreatedBy)
            recipient.DecomposeSender();
    }

    private void AnonymizeParticipants(RecipientInformation recipient, IdentityAddress anonymizedAddress)
    {
        if (recipient.IsRelationshipFullyDecomposed)
            AnonymizeRecipient(recipient.Address, anonymizedAddress);

        if (CanAnonymizeSender)
            AnonymizeSender(anonymizedAddress);
    }

    private void AnonymizeRecipient(IdentityAddress participantAddress, IdentityAddress anonymizedIdentityAddress)
    {
        var recipient = Recipients.FirstOrDefault(r => r.Address == participantAddress);
        recipient?.UpdateAddress(anonymizedIdentityAddress);
    }

    private void AnonymizeSender(IdentityAddress anonymizedIdentityAddress)
    {
        CreatedBy = anonymizedIdentityAddress;
    }

    private bool IsOrphaned(Message message, IdentityAddress anonymizedIdentityAddress)
    {
        return message.CreatedBy == anonymizedIdentityAddress &&
               message.Recipients.All(r => r.Address == anonymizedIdentityAddress);
    }

    #region Expressions

    public static Expression<Func<Message, bool>> HasParticipant(IdentityAddress identityAddress)
    {
        return i =>
            // As soon as the sender has decomposed the relationship to all recipients, the sender should not see the message anymore
            i.CreatedBy == identityAddress && i.Recipients.Any(r => !r.IsRelationshipDecomposedBySender) ||
            // As soon as the recipient has decomposed the relationship to the sender, the recipient should not see the message anymore
            i.Recipients.Any(r => r.Address == identityAddress && !r.IsRelationshipDecomposedByRecipient);
    }

    public static Expression<Func<Message, bool>> WasExchangedBetween(IdentityAddress identityAddress1, IdentityAddress identityAddress2)
    {
        return m =>
            (m.CreatedBy == identityAddress1 && m.Recipients.Any(r => r.Address == identityAddress2)) ||
            (m.CreatedBy == identityAddress2 && m.Recipients.Any(r => r.Address == identityAddress1));
    }

    #endregion
}

public class MessageBody
{
    // ReSharper disable once UnusedMember.Local
    private MessageBody()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        Body = null!;
    }

    public MessageBody(MessageId id, byte[] body)
    {
        Id = id;
        Body = body;
    }

    public MessageId Id { get; }
    public byte[] Body { get; }
}
