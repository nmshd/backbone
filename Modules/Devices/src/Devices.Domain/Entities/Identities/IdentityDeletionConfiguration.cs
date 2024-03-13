namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionConfiguration
{
    public static int MaxApprovalTime { get; set; } = 10;
    public static int LengthOfGracePeriod { get; set; } = 30;
    public static GracePeriodNotificationConfiguration GracePeriodNotification1 { get; set; } = new()
    {
        Time = 20
    };
    public static GracePeriodNotificationConfiguration GracePeriodNotification2 { get; set; } = new()
    {
        Time = 10
    };
    public static GracePeriodNotificationConfiguration GracePeriodNotification3 { get; set; } = new()
    {
        Time = 5
    };
    public static ApprovalReminderNotificationConfiguration ApprovalReminder1 { get; set; } = new()
    {
        Time = 10
    };
    public static ApprovalReminderNotificationConfiguration ApprovalReminder2 { get; set; } = new()
    {
        Time = 5
    };
    public static ApprovalReminderNotificationConfiguration ApprovalReminder3 { get; set; } = new()
    {
        Time = 2
    };

    public static DeletionStartsNotification DeletionStartsNotification { get; set; } = new()
    {
        Text = "After the grace period has passed, your identity's deletion has been processed."
    };
}

public class GracePeriodNotificationConfiguration
{
    public int Time { get; set; }
}

public class ApprovalReminderNotificationConfiguration
{
    public int Time { get; set; }
}

public class DeletionStartsNotification
{
    public string Text { get; set; } = "";
}
