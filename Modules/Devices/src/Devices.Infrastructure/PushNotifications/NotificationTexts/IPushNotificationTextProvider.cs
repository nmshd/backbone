using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;

public interface IPushNotificationTextProvider
{
    Task<(string Title, string Body)> GetNotificationTextForDeviceId(Type pushNotificationType, DeviceId deviceId);
}
