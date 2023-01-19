using System.Linq.Expressions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Devices.Domain.Entities;

public class Device
{
#pragma warning disable CS8618
    private Device() { }
#pragma warning restore CS8618

#pragma warning disable CS8618
    public Device(Identity identity, DeviceId? createdByDevice = null)
#pragma warning restore CS8618
    {
        Id = DeviceId.New();
        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = createdByDevice ?? Id;

        // The following distinction is unfortunatly necessary in order to make EF recognize that the identity already exists
        if (identity.IsNew())
            Identity = identity;
        else
            IdentityAddress = identity.Address;
    }

    public DeviceId Id { get; set; }

    public IdentityAddress IdentityAddress { get; set; }
    public Identity Identity { get; set; }

    public ApplicationUser User { get; set; }

    public DateTime CreatedAt { get; set; }

    public DeviceId CreatedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public DeviceId? DeletedByDevice { get; set; }
    public byte[]? DeletionCertificate { get; set; }

    public static Expression<Func<Device, bool>> IsNotDeleted =>
        device => device.DeletionCertificate == null && device.DeletedAt == null && device.DeletedByDevice == null;

    public void MarkAsDeleted(byte[] deletionCertificate, DeviceId deletedByDevice)
    {
        DeletedAt = SystemTime.UtcNow;
        DeletionCertificate = deletionCertificate;
        DeletedByDevice = deletedByDevice;
    }
}
