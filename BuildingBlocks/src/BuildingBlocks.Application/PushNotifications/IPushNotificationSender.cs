namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotificationSender
{
    Task SendNotification(IPushNotification notification, SendPushNotificationFilter filter, CancellationToken cancellationToken);

    Task SendNotification(IPushNotification notification, SendPushNotificationFilter filter, Dictionary<string, NotificationText> notificationTexts, CancellationToken cancellationToken);

    Task SendTextOnlyNotification(Dictionary<string, NotificationText> notificationTexts, string notificationId, SendPushNotificationFilter filter, CancellationToken cancellationToken);
}
