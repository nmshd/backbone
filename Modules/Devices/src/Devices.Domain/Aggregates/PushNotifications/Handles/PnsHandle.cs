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
                var handle = FcmHandle.Parse(value);
                return handle.IsSuccess
                    ? Result.Success<PnsHandle, DomainError>(handle.Value)
                    : Result.Failure<PnsHandle, DomainError>(DomainErrors.InvalidPnsHandleParse($"Value '{value}' could not be parsed for platform '{platform}'"));
                }
            case PushNotificationPlatform.Apns:
            {
                var handle = ApnsHandle.Parse(value);
                return handle.IsSuccess
                    ? Result.Success<PnsHandle, DomainError>(handle.Value)
                    : Result.Failure<PnsHandle, DomainError>(DomainErrors.InvalidPnsHandleParse($"Value '{value}' could not be parsed for platform '{platform}'"));
                }
            default:
                return Result.Failure<PnsHandle, DomainError>(
                    DomainErrors.InvalidPnsPlatform($"Platform '{platform}' does not exist"));
        }
    }

}
