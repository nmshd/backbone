using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;

public record FcmHandle : PnsHandle
{
    private FcmHandle(PushNotificationPlatform platform, string value) : base(platform, value)
    {
    }

    public static Result<FcmHandle, DomainError> Parse(string value)
    {
        return Result.Success<FcmHandle, DomainError>(new FcmHandle(PushNotificationPlatform.Fcm, value));
    }
}
