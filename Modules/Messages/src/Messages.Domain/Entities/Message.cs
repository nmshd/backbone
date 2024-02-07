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

<<<<<<< Updated upstream
    public string DecryptBody(string symmetricKey)
    {
        return BitConverter.ToString(Body);
    }

    public ConvertibleString DecryptBodyWithSymmetricKey(ConvertibleString encrypted, ConvertibleString symmetricKey)
    {
        var aesEncryptionHelper = AesSymmetricEncrypter.CreateWith96BitIv128BitMac();
        return aesEncryptionHelper.Decrypt(encrypted, symmetricKey);
=======
    public string DecryptBody(string secretKey)
    {
        return Encoding.UTF8.GetString(LibsodiumSymmetricEncrypter.DecryptXChaCha20Poly1305(Body, secretKey));
>>>>>>> Stashed changes
    }
}
