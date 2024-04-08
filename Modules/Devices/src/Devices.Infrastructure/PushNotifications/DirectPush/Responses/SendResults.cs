using System.Collections.Concurrent;
using Backbone.DevelopmentKit.Identity.ValueObjects;

namespace Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush.Responses;

public class SendResults
{
    private readonly BlockingCollection<SendResult> _failures = new();
    private readonly BlockingCollection<SendResult> _successes = new();

    public IEnumerable<SendResult> Failures => _failures;

    public IEnumerable<SendResult> Successes => _successes;

    public void AddFailure(DeviceId deviceId, ErrorReason reason, string message = "")
    {
        _failures.Add(SendResult.Failure(deviceId, reason, message));
    }

    public void AddSuccess(DeviceId deviceId)
    {
        _successes.Add(SendResult.Success(deviceId));
    }
}
