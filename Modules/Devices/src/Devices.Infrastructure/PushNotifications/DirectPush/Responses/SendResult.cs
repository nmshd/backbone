using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.Responses;

public class SendResult
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    public SendError Error { get; private init; } = null!;
    public DeviceId DeviceId { get; private set; } = null!;

    public static SendResult Success(DeviceId deviceId)
    {
        return new SendResult { IsSuccess = true, DeviceId = deviceId };
    }

    public static SendResult Failure(DeviceId deviceId, ErrorReason reason, string message = "")
    {
        return new SendResult { IsSuccess = false, Error = new SendError(reason, message), DeviceId = deviceId };
    }
}
