namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotificationSender
{
    Task SendNotification(IPushNotificationWithConstantText notification, SendPushNotificationFilter filter, CancellationToken cancellationToken);

    Task SendNotification(IPushNotificationWithDynamicText notification, SendPushNotificationFilter filter, Dictionary<string, NotificationText> notificationTexts,
        CancellationToken cancellationToken);
}
