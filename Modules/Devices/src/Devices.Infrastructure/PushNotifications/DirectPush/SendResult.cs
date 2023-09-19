using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class SendResult
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    public SendError Error { get; private init; }

    public enum FailureReason
    {
        InvalidHandle,
        Unexpected
    }

    public class SendError
    {
        public SendError(DeviceId deviceId, FailureReason reason, string message)
        {
            DeviceId = deviceId;
            Reason = reason;
            Message = message;
        }

        public DeviceId DeviceId { get; private set; }
        public FailureReason Reason { get; private set; }
        public string Message { get; private set; }
    }

    public static SendResult Success()
    {
        return new SendResult { IsSuccess = true };
    }

    public static SendResult Failure(DeviceId deviceId, FailureReason reason, string message = "")
    {
        return new SendResult { IsSuccess = false, Error = new SendError(deviceId, reason, message) };
    }
}
