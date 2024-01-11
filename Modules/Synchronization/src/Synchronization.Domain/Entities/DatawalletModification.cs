using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Synchronization.Domain.Entities;

public class DatawalletModification
{
#pragma warning disable CS8618
    private DatawalletModification()
    {
    }
#pragma warning restore CS8618

    public DatawalletModification(Datawallet datawallet, Datawallet.DatawalletVersion datawalletVersion, long index, DatawalletModificationType type, string collection, string objectIdentifier, string payloadCategory, byte[] encryptedPayload, DeviceId createdByDevice, string blobReference)
    {
        Id = DatawalletModificationId.New();

        Datawallet = datawallet;
        DatawalletVersion = datawalletVersion;
        Index = index;

        Type = type;
        Collection = collection;
        ObjectIdentifier = objectIdentifier;
        PayloadCategory = payloadCategory;
        EncryptedPayload = encryptedPayload;
        CreatedBy = datawallet.Owner;

        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = createdByDevice;
        BlobReference = blobReference;
    }

    public DatawalletModificationId Id { get; }
    public Datawallet? Datawallet { get; }
    public Datawallet.DatawalletVersion DatawalletVersion { get; }
    public long Index { get; }
    public string ObjectIdentifier { get; }
    public string? PayloadCategory { get; }
    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }
    public string Collection { get; }
    public DatawalletModificationType Type { get; }
    public byte[]? EncryptedPayload { get; private set; }
    public string BlobReference { get; }

    public void LoadEncryptedPayload(byte[] encryptedPayload)
    {
        if (EncryptedPayload != null)
            throw new Exception("Cannot change the encrypted payload of a datawallet modification.");

        EncryptedPayload = encryptedPayload;
    }
}

public enum DatawalletModificationType
{
    Create = 0,
    Update = 1,
    Delete = 2,
    CacheChanged = 3
}
