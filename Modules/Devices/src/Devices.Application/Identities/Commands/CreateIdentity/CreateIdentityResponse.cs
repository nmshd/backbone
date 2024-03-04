using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Identities.Commands.CreateIdentity;

public class CreateIdentityResponse
{
    public required IdentityAddress Address { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required CreateIdentityResponseDevice Device { get; set; }
}

public class CreateIdentityResponseDevice
{
    public required DeviceId Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
}
