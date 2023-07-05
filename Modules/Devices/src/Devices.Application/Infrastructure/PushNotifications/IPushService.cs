using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;

public interface IPushService
{
    Task UpdateRegistration(IdentityAddress address, DeviceId deviceId, PnsHandle handle, CancellationToken cancellationToken);
    Task SendNotification(IdentityAddress recipient, object notification, CancellationToken cancellationToken);
}
