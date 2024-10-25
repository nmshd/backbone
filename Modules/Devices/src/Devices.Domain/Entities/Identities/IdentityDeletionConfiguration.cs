namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record IdentityDeletionConfiguration
{
    public static IdentityDeletionConfiguration Instance { get; private set; } = new();

    public uint LengthOfApprovalPeriodInDays
    {
        get => (uint)(LengthOfApprovalPeriodInSeconds / 24 / 60 / 60);
        set => LengthOfApprovalPeriodInSeconds = value * 24 * 60 * 60;
    }

    public ulong LengthOfApprovalPeriodInSeconds { get; set; }

    public uint LengthOfGracePeriodInDays
    {
        get => (uint)(LengthOfGracePeriodInSeconds / 24 / 60 / 60);
        set => LengthOfGracePeriodInSeconds = value * 24 * 60 * 60;
    }

    public ulong LengthOfGracePeriodInSeconds { get; set; }

    public GracePeriodNotificationConfiguration GracePeriodNotification1 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 12 };
    public GracePeriodNotificationConfiguration GracePeriodNotification2 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 10 };
    public GracePeriodNotificationConfiguration GracePeriodNotification3 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 5 };

    public ApprovalReminderNotificationConfiguration ApprovalReminder1 { get; set; } = new() { DaysBeforeEndOfApprovalPeriod = 6 };
    public ApprovalReminderNotificationConfiguration ApprovalReminder2 { get; set; } = new() { DaysBeforeEndOfApprovalPeriod = 4 };
    public ApprovalReminderNotificationConfiguration ApprovalReminder3 { get; set; } = new() { DaysBeforeEndOfApprovalPeriod = 2 };
}

public class ApprovalReminderNotificationConfiguration
{
    public ulong SecondsBeforeEndOfApprovalPeriod { get; set; }

    public uint DaysBeforeEndOfApprovalPeriod
    {
        get => (uint)(SecondsBeforeEndOfApprovalPeriod / 24 / 60 / 60);
        set => SecondsBeforeEndOfApprovalPeriod = value * 24 * 60 * 60;
    }
}

public class GracePeriodNotificationConfiguration
{
    public ulong SecondsBeforeEndOfGracePeriod { get; set; }

    public uint DaysBeforeEndOfGracePeriod
    {
        get => (uint)(SecondsBeforeEndOfGracePeriod / 24 / 60 / 60);
        set => SecondsBeforeEndOfGracePeriod = value * 24 * 60 * 60;
    }
}
