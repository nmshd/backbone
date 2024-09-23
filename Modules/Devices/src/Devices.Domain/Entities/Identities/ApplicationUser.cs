using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Tooling;
using Microsoft.AspNetCore.Identity;

namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class ApplicationUser : IdentityUser
{
    private readonly Device _device;

    public ApplicationUser() : base(Username.New())
    {
        _device = null!;
        DeviceId = null!;
        CreatedAt = SystemTime.UtcNow;
    }

    internal ApplicationUser(string username) : base(username)
    {
        _device = null!;
        DeviceId = null!;
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
