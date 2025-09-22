namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public record IdentityDeletionConfiguration
{
    public static IdentityDeletionConfiguration Instance { get; } = new();

    public int AuditLogRetentionPeriodInDays { get; set; } = 731;

    public double LengthOfGracePeriodInDays { get; set; } = 14;

    public GracePeriodNotificationConfiguration GracePeriodNotification1 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 12 };
    public GracePeriodNotificationConfiguration GracePeriodNotification2 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 10 };
    public GracePeriodNotificationConfiguration GracePeriodNotification3 { get; set; } = new() { DaysBeforeEndOfGracePeriod = 5 };
}

public class GracePeriodNotificationConfiguration
{
    public double DaysBeforeEndOfGracePeriod { get; set; }
}
