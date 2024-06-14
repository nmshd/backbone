namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;

public record SseHandle : PnsHandle
{
    private SseHandle(PushNotificationPlatform platform) : base(platform, string.Empty)
    {
    }

    public static SseHandle Create()
    {
        return new SseHandle(PushNotificationPlatform.Sse);
    }
}
