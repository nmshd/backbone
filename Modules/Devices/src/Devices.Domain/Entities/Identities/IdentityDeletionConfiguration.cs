namespace Backbone.Modules.Devices.Domain.Entities.Identities;
public static class IdentityDeletionConfiguration
{
    public static int LengthOfGracePeriod { get; set; } = 30;
    public static int GracePeriodNotification1Time { get; set; } = 20;
    public static int GracePeriodNotification2Time { get; set; } = 10;
    public static int GracePeriodNotification3Time { get; set; } = 5;
}
