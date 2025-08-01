using System.Diagnostics.Metrics;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Responses;

namespace Backbone.Modules.Devices.Infrastructure;

public class PushNotificationMetrics
{
    private readonly Counter<long> _numberOfSentPushNotifications;
    private readonly Counter<long> _numberOfSendErrors;

    public PushNotificationMetrics(Meter meter)
    {
        _numberOfSentPushNotifications = meter.CreateCounter<long>(name: "enmeshed_push_notifications_sent_total");
        _numberOfSendErrors = meter.CreateCounter<long>(name: "enmeshed_push_notifications_send_errors_total");
    }

    public void IncrementNumberOfSentPushNotifications(string? eventName, PushNotificationPlatform platform)
    {
        _numberOfSentPushNotifications.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("platform", platform.ToString())
        );
    }

    public void IncrementNumberOfSendErrors(string? eventName, ErrorReason reason, PushNotificationPlatform platform)
    {
        _numberOfSendErrors.Add(
            1,
            new KeyValuePair<string, object?>("event_name", eventName),
            new KeyValuePair<string, object?>("platform", platform.ToString()),
            new KeyValuePair<string, object?>("reason", reason.ToString())
        );
    }
}
