using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public interface IPushNotificationRegistrationService
{
    Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, Environment environment, CancellationToken cancellationToken);
    Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken);
}

