using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications.UpdateDeviceRegistration;

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
        var mockIdentitiesRepository = A.Fake<IIdentitiesRepository>();

        A.CallTo(() => mockUserContext.GetAddressOrNull())
            .Returns(identity.Address);

        A.CallTo(() => mockUserContext.GetDeviceIdOrNull())
            .Returns(deviceId);

        A.CallTo(() => mockIdentitiesRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._))
            .Returns(identity);

        A.CallTo(() => mockPushService.UpdateRegistration(
                A<IdentityAddress>._,
                A<DeviceId>._,
                A<PnsHandle>._,
                A<string>._,
                A<PushEnvironment>._,
                CancellationToken.None
            ))
            .Returns(DevicePushIdentifier.New());

        var handler = new Handler(mockPushService, mockUserContext, mockIdentitiesRepository);

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
