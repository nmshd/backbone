using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;
using Microsoft.AspNetCore.Identity;

namespace Devices.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    private readonly Device _device;

#pragma warning disable CS8618
    public ApplicationUser() : base(Username.New()) { }
#pragma warning restore CS8618

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

    public void LoginOccurred()
    {
        LastLoginAt = SystemTime.UtcNow;
    }
}
