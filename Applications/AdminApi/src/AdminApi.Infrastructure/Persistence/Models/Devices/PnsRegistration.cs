namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class PnsRegistration
{
    public string DeviceId { get; set; } = null!;
    public PnsHandle Handle { get; set; } = null!;
}

public record PnsHandle
{
    public PushNotificationPlatform Platform { get; }
    public string Value { get; }

    public PnsHandle(PushNotificationPlatform platform, string value)
    {
        Platform = platform;
        Value = value;
    }
}

public enum PushNotificationPlatform
{
    Fcm,
    Apns,
    Sse,
    Dummy
}
