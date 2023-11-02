using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public interface IPushService
{
    Task<DevicePushIdentifier> UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, Environment environment, CancellationToken cancellationToken);
    Task<DevicePushIdentifier> DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken);
    Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken);
}
