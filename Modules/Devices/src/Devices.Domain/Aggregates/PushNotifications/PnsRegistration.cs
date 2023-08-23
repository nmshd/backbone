using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;

public class PnsRegistration
{
    private PnsRegistration() { }

    public PnsRegistration(IdentityAddress identityAddress, DeviceId deviceId, PnsHandle handle, string appId)
    {
        IdentityAddress = identityAddress;
        DeviceId = deviceId;
        Handle = handle;
        UpdatedAt = SystemTime.UtcNow;
        AppId = appId;
    }

    public IdentityAddress IdentityAddress { get; }
    public DeviceId DeviceId { get; }
    public PnsHandle Handle { get; private set; }
    public string? AppId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Update(PnsHandle newHandle, string appId)
    {
        AppId = appId;
        Handle = newHandle;
        UpdatedAt = SystemTime.UtcNow;
    }
}
