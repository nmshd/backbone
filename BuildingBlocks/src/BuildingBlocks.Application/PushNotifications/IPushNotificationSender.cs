using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotificationSender
{
    Task SendNotification(IdentityAddress recipient, IPushNotification notification, CancellationToken cancellationToken);
    Task SendFilteredNotification(IdentityAddress recipient, IPushNotification notification, IEnumerable<string> excludedDevices, CancellationToken cancellationToken);
}
