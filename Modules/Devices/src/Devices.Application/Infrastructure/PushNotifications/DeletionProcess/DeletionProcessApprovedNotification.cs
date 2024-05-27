namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

[NotificationText(Title = "A deletion process has been approved", Body = "One of your identity's deletion processes was approved and will be processed shortly.")]
public record DeletionProcessApprovedNotification(int DaysUntilDeletion);
