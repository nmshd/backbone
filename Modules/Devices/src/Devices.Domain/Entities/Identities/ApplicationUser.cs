using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using Microsoft.AspNetCore.Identity;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class ApplicationUser : IdentityUser
{
    private readonly Device _device;

    public ApplicationUser()
    {
        _device = null!;
        DeviceId = null!;
        CreatedAt = SystemTime.UtcNow;
    }

    internal ApplicationUser(Device device, string username) : base(username)
    {
        _device = device;
        DeviceId = device.Id;
        CreatedAt = SystemTime.UtcNow;
    }

    public ApplicationUser(Device device) : base(Username.New())
    {
        _device = device;
        DeviceId = device.Id;
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

    internal void LoginOccurred()
    {
        LastLoginAt = SystemTime.UtcNow;
    }
}
