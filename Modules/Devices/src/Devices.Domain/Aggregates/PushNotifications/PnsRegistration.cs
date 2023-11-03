using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Tooling;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

public class PnsRegistration
{
    private PnsRegistration() { }

    public PnsRegistration(IdentityAddress identityAddress, DeviceId deviceId, PnsHandle handle, string appId, Environment environment)
    {
        IdentityAddress = identityAddress;
        DeviceId = deviceId;
        DevicePushIdentifier = DevicePushIdentifier.New();
        Handle = handle;
        UpdatedAt = SystemTime.UtcNow;
        AppId = appId;
        Environment = environment;
    }

    public IdentityAddress IdentityAddress { get; }
    public DeviceId DeviceId { get; }
    public DevicePushIdentifier DevicePushIdentifier { get; set; }
    public PnsHandle Handle { get; private set; }
    public string AppId { get; set; }
    public DateTime UpdatedAt { get; private set; }
    public Environment Environment { get; private set; }

    public void Update(PnsHandle newHandle, string appId, Environment environment)
    {
        AppId = appId;
        Handle = newHandle;
        UpdatedAt = SystemTime.UtcNow;
        Environment = environment;
    }
}

public enum Environment
{
    Development,
    Production
}
