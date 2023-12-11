namespace Backbone.Modules.Devices.Domain.Entities.Identities;
public static class IdentityDeletionConfiguration
{
    public static int LengthOfGracePeriod { get; set; } = 30;

    public static GracePeriodNotification GracePeriodNotification1 { get; set; } = new()
    {
        Time = 0,
        Message = string.Empty
    };
    public static GracePeriodNotification GracePeriodNotification2 { get; set; } = new()
    {
        Time = 0,
        Message = string.Empty
    };
    public static GracePeriodNotification GracePeriodNotification3 { get; set; } = new()
    {
        Time = 0,
        Message = string.Empty
    };
}

public class GracePeriodNotification
{
    public int Time { get; set; }
    public string Message { get; set; }
}
