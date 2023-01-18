using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Messages.Domain.Ids;

namespace Messages.Domain.Entities;

public class Message : Identifiable<MessageId>
{
#pragma warning disable CS8618
    private Message() { }
#pragma warning restore CS8618

    public Message(IdentityAddress createdBy, DeviceId createdByDevice, DateTime? doNotSendBefore, byte[] body, IEnumerable<Attachment> attachments, IEnumerable<RecipientInformation> recipients)
    {
        Id = MessageId.New();
        CreatedAt = SystemTime.UtcNow;
        Recipients = recipients.ToList();

        CreatedBy = createdBy;
        CreatedByDevice = createdByDevice;
        DoNotSendBefore = doNotSendBefore;
        Body = body;
        Attachments = attachments.ToList();
    }

    public MessageId Id { get; }

    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }

    public DateTime? DoNotSendBefore { get; }
    public byte[] Body { get; }

    public IReadOnlyCollection<Attachment> Attachments { get; }
    public IReadOnlyCollection<RecipientInformation> Recipients { get; }
}
