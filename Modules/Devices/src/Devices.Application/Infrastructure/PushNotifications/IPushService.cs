using Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Devices.Application.Infrastructure.PushNotifications;

public interface IPushService
{
    Task SendNotificationAsync(IdentityAddress recipient, object notification);
    Task RegisterDeviceAsync(IdentityAddress identityId, DeviceRegistration registration);
}
