namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

[NotificationText(Title = NotificationTextAttribute.DEFAULT_TITLE, Body = "Your Identity will be deleted in X days. You can still cancel up to this point.")]
public record DeletionProcessGracePeriodNotification(int DaysBeforeDeletion);
