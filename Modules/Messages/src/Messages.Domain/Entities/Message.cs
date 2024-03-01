using System.Text.Json;
using Backbone.Crypto;
using Backbone.Crypto.Implementations;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Messages.Domain.Ids;
using Backbone.Tooling;
using Backbone.Tooling.JsonConverters;

namespace Backbone.Modules.Messages.Domain.Entities;

public class Message : IIdentifiable<MessageId>
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

    public string Decrypt(string serializedSecret)
    {
        var secretKey = ExtractSymmetricKey(serializedSecret);

        return new LibsodiumSymmetricEncrypter().Decrypt(
                ConvertibleString.FromByteArray(Body),
                ConvertibleString.FromByteArray(secretKey))
            .Utf8Representation;
    }

    private static byte[] ExtractSymmetricKey(string serializedSecret)
    {
        var deserializedSecret = JsonSerializer.Deserialize<Secret>(serializedSecret,
            new JsonSerializerOptions { Converters = { new UrlSafeBase64ToByteArrayJsonConverter() } });

        var key = Convert.FromBase64String(deserializedSecret!.Key + "=");
        return key;
    }

    private class Secret
    {
        /**
         * Algorithm (`alg`) field is omitted due to not being used in the code.
         */
        public required string Key { get; init; }
    }
}
