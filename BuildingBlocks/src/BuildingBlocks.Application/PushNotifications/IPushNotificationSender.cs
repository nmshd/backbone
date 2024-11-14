using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.BuildingBlocks.Application.PushNotifications;

public interface IPushNotificationSender
{
    // Task SendNotification(IPushNotificationWithConstantTextWithDynamicText notification, CancellationToken cancellationToken);
    // Task SendNotification(IdentityAddress recipient, IPushNotificationWithConstantText notificationWithConstantText, CancellationToken cancellationToken);
    // Task SendFilteredNotification(IdentityAddress recipient, IPushNotificationWithConstantText notificationWithConstantText, IEnumerable<string> excludedDevices, CancellationToken cancellationToken);
    Task SendNotification(IPushNotificationWithConstantText notification, SendPushNotificationFilter filter, CancellationToken cancellationToken);

    Task SendNotification(IPushNotificationWithDynamicText notification, SendPushNotificationFilter filter, Dictionary<string, NotificationText> notificationTexts,
        CancellationToken cancellationToken);
}

public record SendPushNotificationFilter
{
    public List<DeviceId> ExcludedDevices { get; set; } = [];
    public List<IdentityAddress> IncludedIdentities { get; set; } = [];
}

public record NotificationText(string Title, string Body);
