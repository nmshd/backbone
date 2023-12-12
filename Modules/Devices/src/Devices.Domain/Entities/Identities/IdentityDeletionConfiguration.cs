namespace Backbone.Modules.Devices.Domain.Entities.Identities;

public static class IdentityDeletionConfiguration
{
    public static int LengthOfGracePeriod { get; set; } = 30;
    public static int MaxApprovalTime { get; set; } = 5;
    public static IdentityDeletionNotification ApprovalReminder1 { get; set; } = new()
    {
        Time = 0,
        Message = string.Empty
    };
    public static IdentityDeletionNotification ApprovalReminder2 { get; set; } = new()
    {
        Time = 0,
        Message = string.Empty
    };
    public static IdentityDeletionNotification ApprovalReminder3 { get; set; } = new()
    {
        Time = 0,
        Message = string.Empty
    };
}

public class IdentityDeletionNotification
{
    public int Time { get; set; }
    public string Message { get; set; }
}
