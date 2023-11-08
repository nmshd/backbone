using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using FakeItEasy;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;

public class HandlerTests
{
    [Fact]
    public async Task Updating_PnsRegistration_in_PushService()
    {
        // Arrange
        var randomDeviceId = CreateRandomDeviceId();
        var randomIdentity = TestDataGenerator.CreateIdentity();

        var fakeUserContext = A.Fake<IUserContext>();
        var mockPushService = A.Fake<IPushService>();

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(randomIdentity.Address);

        A.CallTo(() => fakeUserContext.GetDeviceIdOrNull())
            .Returns(randomDeviceId);

        A.CallTo(() => mockPushService.UpdateRegistration(
                    A<IdentityAddress>._,
                    A<DeviceId>._,
                    A<PnsHandle>._,
                    A<string>._,
                    A<Environment>._,
                    CancellationToken.None
                ))
            .Returns(DevicePushIdentifier.New());

        var handler = new Handler(mockPushService, fakeUserContext);

        // Act
        var pnsRegistration = await handler.Handle(new UpdateDeviceRegistrationCommand()
        {
            Platform = "fcm",
            Handle = "handle",
            AppId = "keyAppId",
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPushService.UpdateRegistration(
                A<IdentityAddress>._,
                A<DeviceId>._,
                A<PnsHandle>._,
                A<string>._,
                A<Environment>._,
                CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }
}
