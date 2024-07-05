using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Message : Entity, IIdentifiable<MessageId>
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
        Body = body;
        Attachments = attachments.ToList();

        RaiseDomainEvent(new MessageCreatedDomainEvent(this));
    }

    public MessageId Id { get; }

    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; private set; }
    public DeviceId CreatedByDevice { get; }

    public byte[] Body { get; private set; }

    public IReadOnlyCollection<Attachment> Attachments { get; }
    public IReadOnlyCollection<RecipientInformation> Recipients { get; }

    public void LoadBody(byte[] bytes)
    {
        if (Body is { Length: > 0 })
        {
            throw new InvalidOperationException($"The Body of the message {Id} is already filled. It is not possible to change it.");
        }

        Body = bytes;
    }

    public void ReplaceIdentityAddress(IdentityAddress oldIdentityAddress, IdentityAddress newIdentityAddress)
    {
        if (CreatedBy == oldIdentityAddress)
            AnonymizeSender(newIdentityAddress);

        AnonymizeRecipient(oldIdentityAddress, newIdentityAddress);
    }

    public void SanitizeAfterRelationshipDeleted(string participantOne, string participantTwo, IdentityAddress anonymizedIdentityAddress)
    {
        AnonymizeRecipient(participantOne, anonymizedIdentityAddress);
        AnonymizeRecipient(participantTwo, anonymizedIdentityAddress);

        if (CanAnonymizeSender(anonymizedIdentityAddress))
            AnonymizeSender(anonymizedIdentityAddress);
    }

    public static Expression<Func<Message, bool>> HasParticipant(IdentityAddress identityAddress)
    {
        return i => i.CreatedBy == identityAddress || i.Recipients.Any(r => r.Address == identityAddress);
    }

    public static Expression<Func<Message, bool>> WasExchangedBetween(IdentityAddress identityAddress1, IdentityAddress identityAddress2)
    {
        return m =>
            (m.CreatedBy == identityAddress1 && m.Recipients.Any(r => r.Address == identityAddress2)) ||
            (m.CreatedBy == identityAddress2 && m.Recipients.Any(r => r.Address == identityAddress1));
    }

    private void AnonymizeRecipient(string participantAddress, IdentityAddress anonymizedIdentityAddress)
    {
        var recipient = Recipients.FirstOrDefault(r => r.Address == participantAddress);
        recipient?.UpdateAddress(anonymizedIdentityAddress);
    }

    private bool CanAnonymizeSender(IdentityAddress anonymizedIdentityAddress)
    {
        return Recipients.All(r => r.Address == anonymizedIdentityAddress);
    }

    private void AnonymizeSender(IdentityAddress anonymizedIdentityAddress)
    {
        CreatedBy = anonymizedIdentityAddress;
    }
}
