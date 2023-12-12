namespace Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;

[NotificationText(Title = "Deletion process waiting for approval.",
    Body = "There is a deletion process for your identity that waits for your approval. If you don't approve it within a few days, it will be closed.")]
public record DeletionProcessWaitingForApprovalReminderPushNotification(int DaysUntilApprovalPeriodEnds);
