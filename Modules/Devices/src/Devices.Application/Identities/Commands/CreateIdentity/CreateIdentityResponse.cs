using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class CreateIdentityResponse
{
    public IdentityAddress? Address { get; set; }
    public DateTime CreatedAt { get; set; }

    public CreateIdentityResponseDevice? Device { get; set; }
}

public class CreateIdentityResponseDevice
{
    public DeviceId? Id { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }
}
