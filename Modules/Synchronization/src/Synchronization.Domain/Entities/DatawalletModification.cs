using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using static Backbone.Modules.Synchronization.Domain.Entities.Datawallet;

namespace Backbone.Modules.Synchronization.Domain.Entities;

public class DatawalletModification : Entity
{
    [UsedImplicitly(Reason = "This constructor is for EF Core only")]
    protected DatawalletModification()
    {
        Id = null!;
        Datawallet = null!;
        DatawalletVersion = null!;
        ObjectIdentifier = null!;
        CreatedBy = null!;
        CreatedByDevice = null!;
        Collection = null!;
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
        EncryptedPayload = encryptedPayload;
        CreatedBy = datawallet.Owner;

        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = createdByDevice;
    }

    public DatawalletModificationId Id { get; }
    public virtual Datawallet Datawallet { get; }
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

    public static Expression<Func<DatawalletModification, bool>> CanBeCleanedUp =>
        currentMod =>
            // older than 30 days
            currentMod.CreatedAt <= SystemTime.UtcNow.AddDays(-30) &&
            currentMod.Datawallet.Modifications.Any(otherMod =>
                // ignore itself
                otherMod.Id != currentMod.Id &&
                // it's not the latest one
                otherMod.Index > currentMod.Index &&
                otherMod.Collection == currentMod.Collection &&
                otherMod.ObjectIdentifier == currentMod.ObjectIdentifier &&
                (
                    // a DELETE modification exists
                    otherMod.Type == DatawalletModificationType.Delete
                    ||
                    // a CREATE or UPDATE modification for the same payload category exists, and the current modification is also a CREATE or UPDATE
                    (
                        (otherMod.Type == DatawalletModificationType.Update || otherMod.Type == DatawalletModificationType.Create) &&
                        (currentMod.Type == DatawalletModificationType.Update || currentMod.Type == DatawalletModificationType.Create) &&
                        otherMod.PayloadCategory == currentMod.PayloadCategory
                    )
                )
            );
}

public enum DatawalletModificationType
{
    Create = 0,
    Update = 1,
    Delete = 2,
    CacheChanged = 3
}
