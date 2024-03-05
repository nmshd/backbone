namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

[NotificationText(Title = "Your Identity will be deleted", Body = "Your Identity will be deleted in a few days. You can still cancel up to this point.")]
public record DeletionProcessGracePeriodNotification(int DaysUntilDeletion);
