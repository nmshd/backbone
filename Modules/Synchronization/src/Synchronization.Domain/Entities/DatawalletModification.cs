using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Domain.Entities;

public class DatawalletModification
{
    // ReSharper disable once UnusedMember.Local
    private DatawalletModification()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        DatawalletVersion = null!;
        ObjectIdentifier = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Collection = null!;
        BlobReference = null!;
    }

    public DatawalletModification(Datawallet datawallet, DatawalletVersion datawalletVersion, long index, DatawalletModificationType type, string collection, string objectIdentifier, string? payloadCategory, byte[]? encryptedPayload, DeviceId createdByDevice, string blobReference)
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
    public DatawalletVersion DatawalletVersion { get; }
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
