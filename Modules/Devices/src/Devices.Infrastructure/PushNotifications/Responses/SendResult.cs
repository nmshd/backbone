using System.Diagnostics.CodeAnalysis;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

public class SendResult
{
    private SendResult(DeviceId deviceId)
    {
        DeviceId = deviceId;
        IsSuccess = true;
    }

    private SendResult(DeviceId deviceId, SendError error)
    {
        DeviceId = deviceId;
        Error = error;
        IsSuccess = false;
    }

    public bool IsSuccess { get; }

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    public DeviceId DeviceId { get; private set; }
    public SendError? Error { get; private init; }

    public static SendResult Success(DeviceId deviceId)
    {
        return new SendResult(deviceId);
    }

    public static SendResult Failure(DeviceId deviceId, ErrorReason reason, string? message = null)
    {
        return new SendResult(deviceId, new SendError(reason, message));
    }
}
