namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record IdentityDeletionConfiguration
{
    public static IdentityDeletionConfiguration Instance { get; private set; } = new();

    public double LengthOfApprovalPeriodInDays { get; set; } = 7;
    public double LengthOfGracePeriodInDays { get; set; } = 14;

    public GracePeriodNotificationConfiguration GracePeriodNotification1 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 12 };
    public GracePeriodNotificationConfiguration GracePeriodNotification2 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 10 };
    public GracePeriodNotificationConfiguration GracePeriodNotification3 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 5 };

    public ApprovalReminderNotificationConfiguration ApprovalReminder1 { get; set; } = new() { DaysBeforeEndOfApprovalPeriod = 6 };
    public ApprovalReminderNotificationConfiguration ApprovalReminder2 { get; set; } = new() { DaysBeforeEndOfApprovalPeriod = 4 };
    public ApprovalReminderNotificationConfiguration ApprovalReminder3 { get; set; } = new() { DaysBeforeEndOfApprovalPeriod = 2 };
}

public class ApprovalReminderNotificationConfiguration
{
    public uint DaysBeforeEndOfApprovalPeriod { get; set; }
}

public class GracePeriodNotificationConfiguration
{
    public uint DaysBeforeEndOfGracePeriod { get; set; }
}
