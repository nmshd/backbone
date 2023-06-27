using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
public record class FcmHandle : PnsHandle
{
    public FcmHandle(PushNotificationPlatform platform, string value) : base(platform, value)
    {
    }

    public static Result<FcmHandle, DomainError> Parse(string value)
    {
        return Result.Success<FcmHandle, DomainError>(new FcmHandle(PushNotificationPlatform.Fcm, value));
    }
}
