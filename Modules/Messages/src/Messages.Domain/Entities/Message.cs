using System.Text;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Message : IIdentifiable<MessageId>
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

    public string Decrypt(string secretKey)
    {
        return new LibsodiumSymmetricEncrypter().Decrypt(
                ConvertibleString.FromByteArray(Body),
                ConvertibleString.FromUtf8(secretKey))
            .Utf8Representation;
    }
}
