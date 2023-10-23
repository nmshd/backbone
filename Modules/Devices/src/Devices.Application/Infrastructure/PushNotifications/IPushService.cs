using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Devices.Domain.Aggregates.PushNotifications.Handles;

namespace Backbone.Devices.Application.Infrastructure.PushNotifications;

public interface IPushService
{
    Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, string appId, CancellationToken cancellationToken);
    Task DeleteRegistration(DeviceId deviceId, CancellationToken cancellationToken);
    Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken);
}
