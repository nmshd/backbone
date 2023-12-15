namespace Backbone.Modules.Devices.Domain.Entities.Identities;
public static class IdentityDeletionConfiguration
{
    public static int LengthOfGracePeriod { get; set; } = 30;

    public static GracePeriodNotification GracePeriodNotification1 { get; set; } = new()
    {
        Time = 20,
        Message = "Your Identity will be deleted in a few days. You can still cancel up to this point."
    };
    public static GracePeriodNotification GracePeriodNotification2 { get; set; } = new()
    {
        Time = 10,
        Message = "Your Identity will be deleted in a few days. You can still cancel up to this point."
    };
    public static GracePeriodNotification GracePeriodNotification3 { get; set; } = new()
    {
        Time = 5,
        Message = "Your Identity will be deleted in a few days. You can still cancel up to this point."
    };
}

public class GracePeriodNotification
{
    public int Time { get; set; }
    public string Message { get; set; }
}
