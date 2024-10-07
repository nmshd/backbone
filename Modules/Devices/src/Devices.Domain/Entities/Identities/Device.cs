using System.Linq.Expressions;
using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.Errors;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class Device : Entity
{
    // ReSharper disable once UnusedMember.Local
    private Device()
    {
        // This constructor is for EF Core only; initializing the properties with null is therefore not a problem
        Id = null!;
        IdentityAddress = null!;
        Identity = null!;
        User = null!;
        CreatedByDevice = null!;
        CommunicationLanguage = null!;
    }

    private Device(Identity identity, CommunicationLanguage communicationLanguage, string username)
    {
        Id = DeviceId.New();
        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = Id;
        CommunicationLanguage = communicationLanguage;

        User = new ApplicationUser(this, username);

        Identity = identity;
        IdentityAddress = null!;
    }

    public Device(Identity identity, CommunicationLanguage communicationLanguage, DeviceId? createdByDevice = null)
    {
        Id = DeviceId.New();
        CreatedAt = SystemTime.UtcNow;
        CreatedByDevice = createdByDevice ?? Id;
        CommunicationLanguage = communicationLanguage;

        User = new ApplicationUser(this);

        // The following distinction is unfortunately necessary in order to make EF recognize that the identity already exists
        if (identity.IsNew())
        {
            Identity = identity;
            IdentityAddress = null!; // This is just to satisfy the compiler; the property is actually set by EF core
        }
        else
        {
            Identity = null!; // This is just to satisfy the compiler; the property is actually set by EF core
            IdentityAddress = identity.Address;
        }
    }

    public DeviceId Id { get; set; }

    public IdentityAddress IdentityAddress { get; set; }
    public Identity Identity { get; set; }

    public ApplicationUser User { get; set; }

    public DateTime CreatedAt { get; set; }

    public CommunicationLanguage CommunicationLanguage { get; private set; }

    public DeviceId CreatedByDevice { get; set; }

    public DateTime? DeletedAt { get; set; }
    public DeviceId? DeletedByDevice { get; set; }

    public bool IsOnboarded => User.HasLoggedIn;

    public static Expression<Func<Device, bool>> IsNotDeleted =>
        device => device.DeletedAt == null && device.DeletedByDevice == null;

    private DomainError? CanBeDeletedBy(IdentityAddress addressOfActiveIdentity)
    {
        if (IsOnboarded)
            return new DomainError("error.platform.validation.device.deviceCannotBeDeleted", "The device cannot be deleted because it is already onboarded.");

        if (IdentityAddress != addressOfActiveIdentity)
            return new DomainError("error.platform.validation.device.deviceCannotBeDeleted", "You are not allowed to delete this device as it belongs to another identity.");

        return null;
    }

    public bool Update(CommunicationLanguage communicationLanguage)
    {
        var hasChanges = false;
        if (communicationLanguage != CommunicationLanguage)
        {
            hasChanges = true;
            CommunicationLanguage = communicationLanguage;
        }

        return hasChanges;
    }

    public void MarkAsDeleted(DeviceId deletedByDevice, IdentityAddress addressOfActiveIdentity)
    {
        var error = CanBeDeletedBy(addressOfActiveIdentity);

        if (error != null)
            throw new DomainException(error);

        DeletedAt = SystemTime.UtcNow;
        DeletedByDevice = deletedByDevice;
    }

    public static Device CreateTestDevice(Identity identity, CommunicationLanguage communicationLanguage, string username)
    {
        return new Device(identity, communicationLanguage, username);
    }
}
