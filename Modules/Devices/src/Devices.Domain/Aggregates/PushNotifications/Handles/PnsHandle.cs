using CSharpFunctionalExtensions;
using Enmeshed.BuildingBlocks.Domain.Errors;

namespace Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
public record PnsHandle
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
        switch (platform)
        {
            case PushNotificationPlatform.Fcm:
                {
                    var parseHandleResult = FcmHandle.Parse(value);
                    return parseHandleResult.IsSuccess
                        ? Result.Success<PnsHandle, DomainError>(parseHandleResult.Value)
                        : Result.Failure<PnsHandle, DomainError>(parseHandleResult.Error);
                }
            case PushNotificationPlatform.Apns:
                {
                    var parseHandleResult = ApnsHandle.Parse(value);
                    return parseHandleResult.IsSuccess
                        ? Result.Success<PnsHandle, DomainError>(parseHandleResult.Value)
                        : Result.Failure<PnsHandle, DomainError>(parseHandleResult.Error);
                }
            default:
                return Result.Failure<PnsHandle, DomainError>(
                    DomainErrors.InvalidPnsPlatform($"Platform '{platform}' does not exist"));
        }
    }

}
