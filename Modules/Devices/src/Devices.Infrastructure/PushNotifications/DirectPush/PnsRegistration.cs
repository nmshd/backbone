using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Enmeshed.Tooling;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
public class PnsRegistration
{
    private PnsRegistration() {}

    public PnsRegistration(IdentityAddress identityAddress, DeviceId deviceId, PnsHandle handle)
    {
        IdentityAddress = identityAddress;
        DeviceId = deviceId;
        Handle = handle;
        UpdatedAt = SystemTime.UtcNow;
    }

    public IdentityAddress IdentityAddress { get; }
    public DeviceId DeviceId { get; }
    public PnsHandle Handle { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void Update(PnsHandle newHandle)
    {
        Handle = newHandle;
        UpdatedAt = SystemTime.UtcNow;
    }
}
