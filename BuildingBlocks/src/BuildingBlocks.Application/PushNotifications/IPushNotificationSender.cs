namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotificationSender
{
    Task SendNotification(IPushNotification notification, SendPushNotificationFilter filter, CancellationToken cancellationToken);

    Task SendNotification(IPushNotification notification, Dictionary<string, NotificationText> notificationTexts, SendPushNotificationFilter filter, CancellationToken cancellationToken);

    Task SendNotification(string notificationId, Dictionary<string, NotificationText> notificationTexts, SendPushNotificationFilter filter, CancellationToken cancellationToken);
}
