using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Domain.Entities;

public class DatawalletModification : Entity
{
    // ReSharper disable once UnusedMember.Local
    protected DatawalletModification()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        DatawalletVersion = null!;
        ObjectIdentifier = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Collection = null!;
        Details = null!;
    }

    public DatawalletModification(Datawallet datawallet, DatawalletVersion datawalletVersion, long index, DatawalletModificationType type, string collection, string objectIdentifier,
        string? payloadCategory, byte[]? encryptedPayload, DeviceId createdByDevice)
    {
        Id = DatawalletModificationId.New();
        Datawallet = datawallet;
        DatawalletVersion = datawalletVersion;
        Index = index;

        Type = type;
        Collection = collection;
        ObjectIdentifier = objectIdentifier;
        PayloadCategory = payloadCategory;
        Details = new DatawalletModificationDetails { Id = Id, EncryptedPayload = encryptedPayload };
        CreatedBy = datawallet.Owner;

        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = createdByDevice;
    }

    public DatawalletModificationId Id { get; }
    public virtual Datawallet? Datawallet { get; }
    public DatawalletVersion DatawalletVersion { get; }
    public long Index { get; }
    public string ObjectIdentifier { get; }
    public string? PayloadCategory { get; }
    public DateTime CreatedAt { get; }
    public IdentityAddress CreatedBy { get; }
    public DeviceId CreatedByDevice { get; }
    public string Collection { get; }
    public DatawalletModificationType Type { get; }
    public virtual DatawalletModificationDetails Details { get; }
}

public enum DatawalletModificationType
{
    Create = 0,
    Update = 1,
    Delete = 2,
    CacheChanged = 3
}

public class DatawalletModificationDetails
{
    public required DatawalletModificationId Id { get; init; } = null!;
    public required byte[]? EncryptedPayload { get; init; }
}
