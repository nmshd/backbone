using System.Collections.Concurrent;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;

public class SendResult
{
    public bool IsSuccess { get; private init; }
    public bool IsFailure => !IsSuccess;
    public SendError Error { get; private init; }
    public DeviceId DeviceId { get; private set; }

    public enum ErrorReason
    {
        InvalidHandle,
        Unexpected
    }

    public class SendError
    {
        public SendError(ErrorReason reason, string message)
        {
            Reason = reason;
            Message = message;
        }

        public ErrorReason Reason { get; private set; }
        public string Message { get; private set; }
    }

    public static SendResult Success(DeviceId deviceId)
    {
        return new SendResult { IsSuccess = true, DeviceId = deviceId };
    }

    public static SendResult Failure(DeviceId deviceId, ErrorReason reason, string message = "")
    {
        return new SendResult { IsSuccess = false, Error = new SendError(reason, message), DeviceId = deviceId };
    }
}

public class SendResults
{
    private readonly BlockingCollection<SendResult> _failures = new();
    private readonly BlockingCollection<SendResult> _successes = new();

    public IEnumerable<SendResult> Failures => _failures;

    public IEnumerable<SendResult> Successes => _successes;

    public void AddFailure(DeviceId deviceId, SendResult.ErrorReason reason, string message = "")
    {
        _failures.Add(SendResult.Failure(deviceId, reason, message));
    }

    public void AddSuccess(DeviceId deviceId)
    {
        _successes.Add(SendResult.Success(deviceId));
    }
}
