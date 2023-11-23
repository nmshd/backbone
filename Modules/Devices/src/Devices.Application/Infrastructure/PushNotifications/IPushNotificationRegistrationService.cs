using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public interface IPushNotificationRegistrationService
{
    Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, PushEnvironment environment, CancellationToken cancellationToken);
    Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken);
}

