using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Synchronization.Domain.Entities;

public class Datawallet
{
#pragma warning disable CS8618
    private Datawallet() { }
#pragma warning restore CS8618

    public Datawallet(DatawalletVersion version, IdentityAddress owner) : this()
    {
        Id = DatawalletId.New();
        Version = version;
        Owner = owner;
        Modifications = new List<DatawalletModification>();
    }

    public DatawalletId Id { get; }
    public IdentityAddress Owner { get; }
    public DatawalletVersion Version { get; private set; }
    public List<DatawalletModification> Modifications { get; }
    public DatawalletModification? LatestModification => Modifications.MaxBy(m => m.Index);

    public void Upgrade(DatawalletVersion targetVersion)
    {
        if (targetVersion < Version)
            throw new DomainException($"You cannot upgrade from version '{Version}' to '{targetVersion}', because it is not possible to upgrade to lower versions.");

        Version = targetVersion;
    }

    public DatawalletModification AddModification(DatawalletModificationType type, DatawalletVersion datawalletVersion, string collection, string objectIdentifier, string payloadCategory, byte[] encryptedPayload, DeviceId createdByDevice)
    {
        if (datawalletVersion > Version)
            throw new DomainException($"Cannot add modifications with DatawalletVersion '{datawalletVersion}', because the datawallet only has version '{Version}'.");

        var indexOfNewModification = Modifications.Count > 0 ? Modifications.Max(m => m.Index) + 1 : 0;
        var newModification = new DatawalletModification(this, datawalletVersion, indexOfNewModification, type, collection, objectIdentifier, payloadCategory, encryptedPayload, createdByDevice);
        Modifications.Add(newModification);
        return newModification;
    }

    public DatawalletModification AddModification(DatawalletModification modification)
    {
        Modifications.Add(modification);
        return modification;
    }

    public class DatawalletVersion : SimpleValueObject<ushort>
    {
        public DatawalletVersion(ushort value) : base(value) { }

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
