// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Backbone.AdminApi.Infrastructure.Persistence.Models.Devices;

public class PnsRegistration
{
    public required string DeviceId { get; init; }
    public required PnsHandle Handle { get; init; }
}

public record PnsHandle
{
    public PushNotificationPlatform Platform { get; init; }
    public string Value { get; init; }

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
