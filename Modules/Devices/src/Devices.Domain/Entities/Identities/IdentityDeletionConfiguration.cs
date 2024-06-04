namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public class IdentityDeletionConfiguration
{
    public static int LengthOfApprovalPeriod { get; } = 7;
    public static int LengthOfGracePeriod { get; } = 14;

    public static GracePeriodNotificationConfiguration GracePeriodNotification1 { get; } = new()
    {
        Time = 12
    };

    public static GracePeriodNotificationConfiguration GracePeriodNotification2 { get; } = new()
    {
        Time = 10
    };

    public static GracePeriodNotificationConfiguration GracePeriodNotification3 { get; } = new()
    {
        Time = 5
    };

    public static ApprovalReminderNotificationConfiguration ApprovalReminder1 { get; } = new()
    {
        Time = 6
    };

    public static ApprovalReminderNotificationConfiguration ApprovalReminder2 { get; } = new()
    {
        Time = 4
    };

    public static ApprovalReminderNotificationConfiguration ApprovalReminder3 { get; } = new()
    {
        Time = 2
    };
}

public class GracePeriodNotificationConfiguration
{
    public int Time { get; init; }
}

public class ApprovalReminderNotificationConfiguration
{
    public int Time { get; init; }
}

public class DeletionStartsNotification
{
    public string Text { get; init; } = "";
}
