using Backbone.Modules.Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public interface IPushService
{
    Task SendNotificationAsync(IdentityAddress recipient, object notification);
    Task RegisterDeviceAsync(IdentityAddress identityId, DeviceRegistration registration);
}
