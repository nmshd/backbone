using Backbone.Modules.Devices.Abstractions.PushNotifications;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared;
using Backbone.Modules.Devices.Module.Features.PushNotifications.UpdateDeviceRegistration;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using FakeItEasy;

namespace Backbone.Modules.Devices.Module.Tests.Tests.PushNotifications.UpdateDeviceRegistration;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Updating_PnsRegistration_in_PushService()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identity = TestDataGenerator.CreateIdentity();

        var mockUserContext = A.Fake<IUserContext>();
        var mockPushService = A.Fake<IPushNotificationRegistrationService>();

        A.CallTo(() => mockUserContext.GetAddressOrNull())
            .Returns(identity.Address);

        A.CallTo(() => mockUserContext.GetDeviceIdOrNull())
            .Returns(deviceId);

        A.CallTo(() => mockPushService.UpdateRegistration(
                A<IdentityAddress>._,
                A<DeviceId>._,
                A<PnsHandle>._,
                A<string>._,
                A<PushEnvironment>._,
                CancellationToken.None
            ))
            .Returns(DevicePushIdentifier.New());

        var handler = new Handler(mockPushService, mockUserContext);

        // Act
        await handler.Handle(new UpdateDeviceRegistrationCommand
        {
            Platform = "fcm",
            Handle = "handle",
            AppId = "someAppId",
            Environment = "Development"
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushService.UpdateRegistration(
                mockUserContext.GetAddress(),
                mockUserContext.GetDeviceId(),
                PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value,
                "someAppId",
                PushEnvironment.Development,
                CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }
}
