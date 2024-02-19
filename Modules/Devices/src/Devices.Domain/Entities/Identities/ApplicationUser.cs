using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using Microsoft.AspNetCore.Identity;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class ApplicationUser : IdentityUser
{
    private readonly Device _device;

    // This constructor is required by AspnetCoreIdentity
    public ApplicationUser()
    {
        _device = null!;
        DeviceId = null!;
    }

    public ApplicationUser(Device device) : base(Username.New())
    {
        _device = device;
        DeviceId = null!;
    }

    public ApplicationUser(Identity identity, DeviceId? createdByDevice = null) : base(Username.New())
    {
        _device = new Device(identity, createdByDevice);
        DeviceId = Device.Id;

        CreatedAt = SystemTime.UtcNow;
    }

    public DeviceId DeviceId { get; set; }

    public Device Device
    {
        get => _device;
        init
        {
            _device = value;
            DeviceId = value.Id;
        }
    }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public bool HasLoggedIn => LastLoginAt.HasValue;

    public void LoginOccurred()
    {
        LastLoginAt = SystemTime.UtcNow;
    }
}
