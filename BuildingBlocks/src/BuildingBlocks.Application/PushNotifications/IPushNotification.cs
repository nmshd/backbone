namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotification;

public static class IPushNotificationExtensions
{
    private const string PUSH_NOTIFICATION_POSTFIX = "PushNotification";

    public static string GetEventName(this IPushNotification pushNotification)
    {
        var notificationTypeName = pushNotification.GetType().Name;

        if (notificationTypeName.Contains(PUSH_NOTIFICATION_POSTFIX))
            return notificationTypeName.Replace(PUSH_NOTIFICATION_POSTFIX, "");

        return "dynamic";
    }
}
