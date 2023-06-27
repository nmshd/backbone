using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
public record class PnsHandle
{
    public PushNotificationPlatform Platform { get; }
    public string Value { get; }

    protected PnsHandle(PushNotificationPlatform platform, string value)
    {
        Platform = platform;
        Value = value;
    }

    public static Result<PnsHandle, DomainError> Parse(string value, PushNotificationPlatform platform)
    {
        return platform switch
        {
            PushNotificationPlatform.Fcm => Result.Success<PnsHandle, DomainError>(FcmHandle.Parse(value).Value),
            PushNotificationPlatform.Apns => Result.Success<PnsHandle, DomainError>(ApnsHandle.Parse(value).Value),
            _ => Result.Failure<PnsHandle, DomainError>(DomainErrors.InvalidPnsPlatform($"Platform {platform} does not exist")),
        };
    }

}
