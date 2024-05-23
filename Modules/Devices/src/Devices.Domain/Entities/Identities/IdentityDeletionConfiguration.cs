namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionConfiguration
{
    public static int LengthOfApprovalPeriod { get; set; } = 7;
    public static int LengthOfGracePeriod { get; set; } = 14;

    public static GracePeriodNotificationConfiguration GracePeriodNotification1 { get; set; } = new()
    {
        Time = 12
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
        Time = 6
    };

    public static ApprovalReminderNotificationConfiguration ApprovalReminder2 { get; set; } = new()
    {
        Time = 4
    };

    public static ApprovalReminderNotificationConfiguration ApprovalReminder3 { get; set; } = new()
    {
        Time = 2
    };

    public static DeletionStartsNotification DeletionStartsNotification { get; set; } = new()
    {
        Text = "The grace period for the deletion of your identity has expired. The deletion starts now."
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
