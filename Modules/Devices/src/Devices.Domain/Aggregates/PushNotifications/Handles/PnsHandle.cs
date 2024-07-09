using Backbone.BuildingBlocks.Domain.Errors;
using CSharpFunctionalExtensions;

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

    public static Result<PnsHandle, DomainError> Parse(PushNotificationPlatform platform, string value)
    {
        switch (platform)
        {
            case PushNotificationPlatform.Fcm:
            {
                var parseHandleResult = FcmHandle.Parse(value);
                return parseHandleResult.Map(v => (PnsHandle)v);
            }
            case PushNotificationPlatform.Apns:
            {
                var parseHandleResult = ApnsHandle.Parse(value);
                return parseHandleResult.Map(v => (PnsHandle)v);
            }
            case PushNotificationPlatform.Sse:
            {
                var handle = SseHandle.Create();
                return Result.Success<PnsHandle, DomainError>(handle);
            }
            case PushNotificationPlatform.Dummy:
            {
                var parseHandleResult = DummyHandle.Create();
                return parseHandleResult.Map(v => (PnsHandle)v);
            }
            default:
                return Result.Failure<PnsHandle, DomainError>(DomainErrors.InvalidPnsPlatform($"Platform '{platform}' does not exist"));
        }
    }
}

public record DummyHandle : PnsHandle
{
    private DummyHandle() : base(PushNotificationPlatform.Dummy, "")
    {
    }

    public static Result<DummyHandle, DomainError> Create()
    {
        return Result.Success<DummyHandle, DomainError>(new DummyHandle());
    }
}
