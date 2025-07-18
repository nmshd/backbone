namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

public class SendError
{
    public SendError(ErrorReason reason, string? message)
    {
        Reason = reason;
        Message = message ?? "";
    }

    public ErrorReason Reason { get; private set; }
    public string Message { get; private set; }
}
