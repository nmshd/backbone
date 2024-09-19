using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;

public record ApnsHandle : PnsHandle
{
    private ApnsHandle(PushNotificationPlatform platform, string value) : base(platform, value)
    {
    }

    public static Result<ApnsHandle, DomainError> Parse(string value)
    {
        return Result.Success<ApnsHandle, DomainError>(new ApnsHandle(PushNotificationPlatform.Apns, value));
    }
}
