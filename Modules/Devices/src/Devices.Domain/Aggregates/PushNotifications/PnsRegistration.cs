using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Tooling;
using Backbone.Tooling.Extensions;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

public class PnsRegistration
{
    // ReSharper disable once UnusedMember.Local
    private PnsRegistration() { }

    public PnsRegistration(IdentityAddress identityAddress, DeviceId deviceId, PnsHandle handle, string appId, PushEnvironment environment)
    {
        IdentityAddress = identityAddress;
        DeviceId = deviceId;
        DevicePushIdentifier = DevicePushIdentifier.New();
        Handle = handle;
        UpdatedAt = SystemTime.UtcNow;
        AppId = appId;
        Environment = environment;
    }

    public IdentityAddress IdentityAddress { get; } = null!;
    public DeviceId DeviceId { get; } = null!;
    public DevicePushIdentifier DevicePushIdentifier { get; private set; } = null!;
    public PnsHandle Handle { get; private set; } = null!;
    public string AppId { get; private set; } = null!;
    public DateTime UpdatedAt { get; private set; }
    public PushEnvironment Environment { get; private set; }

    public void Update(PnsHandle newHandle, string appId, PushEnvironment environment)
    {
        AppId = appId;
        Handle = newHandle;
        UpdatedAt = SystemTime.UtcNow;
        Environment = environment;

        // this may be the case for old registrations that were created before the introduction of DevicePushIdentifiers, because the identifier column has a default value of an empty string
        if (DevicePushIdentifier.StringValue.Trim().IsEmpty())
            DevicePushIdentifier = DevicePushIdentifier.New();
    }
}

public enum PushEnvironment
{
    Development,
    Production
}
