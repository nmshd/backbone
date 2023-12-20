namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public static class IdentityDeletionConfiguration
{
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

    public static DeletionStartsNotification DeletionStartsNotification { get; set; } = new();
}

public class GracePeriodNotificationConfiguration
{
    public int Time { get; set; }
}

public class DeletionStartsNotification
{
    public string Message { get; set; } = "";
}
