using Backbone.BuildingBlocks.Domain;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;

namespace Backbone.Modules.Synchronization.Domain.Entities;

public class Datawallet : Entity
{
    // ReSharper disable once UnusedMember.Local
    private Datawallet()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        Owner = null!;
        Version = null!;
        Modifications = null!;
    }

    public Datawallet(DatawalletVersion version, IdentityAddress owner)
    {
        Id = DatawalletId.New();
        Version = version;
        Owner = owner;
        Modifications = [];
    }

    public DatawalletId Id { get; }
    public IdentityAddress Owner { get; }
    public DatawalletVersion Version { get; private set; }
    public List<DatawalletModification> Modifications { get; }
    public DatawalletModification? LatestModification => Modifications.MaxBy(m => m.Index);

    public void Upgrade(DatawalletVersion targetVersion)
    {
        if (targetVersion < Version)
            throw new DomainException(DomainErrors.Datawallet.CannotDowngrade(Version.Value, targetVersion.Value));

        Version = targetVersion;
    }

    public DatawalletModification AddModification(DatawalletModificationType type, DatawalletVersion datawalletVersionOfModification, string collection, string objectIdentifier,
        string? payloadCategory, byte[]? encryptedPayload, DeviceId createdByDevice, string blobReference)
    {
        if (datawalletVersionOfModification > Version)
            throw new DomainException(DomainErrors.Datawallet.DatawalletVersionOfModificationTooHigh(Version, datawalletVersionOfModification));

        var indexOfNewModification = Modifications.Count > 0 ? Modifications.Max(m => m.Index) + 1 : 0;

        var newModification = new DatawalletModification(this, datawalletVersionOfModification, indexOfNewModification, type, collection, objectIdentifier, payloadCategory, encryptedPayload,
            createdByDevice, blobReference);
        Modifications.Add(newModification);

        RaiseDomainEvent(new DatawalletModifiedDomainEvent(Owner, createdByDevice));

        return newModification;
    }

    public class DatawalletVersion : SimpleValueObject<ushort>
    {
        public DatawalletVersion(ushort value) : base(value)
        {
        }

        public static bool operator <(DatawalletVersion versionA, DatawalletVersion versionB)
        {
            return versionA.Value < versionB.Value;
        }

        public static bool operator >(DatawalletVersion versionA, DatawalletVersion versionB)
        {
            return versionA.Value > versionB.Value;
        }
    }
}
