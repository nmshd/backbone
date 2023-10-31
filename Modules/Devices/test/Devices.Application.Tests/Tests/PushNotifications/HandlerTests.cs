using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Hashing;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications;

public class HandlerTests
{
    // now DirectPushService returns Task to Handler, Handler returns Task to Controller, Controller returns Task to Client
    // todo: DirectPushService returns DirectPushIdentifier, and so on...

    [Fact]
    public async Task Adding_a_new_PNS_registration()
    {
        // Arrange
        var randomDeviceId = TestDataGenerator.CreateRandomDeviceId();
        var identity = TestDataGenerator.CreateIdentity();
        var directPushIdentifier = DevicePushIdentifier.Create(randomDeviceId); // todo: will be used later

        var mockPnsRegistrationRepository = A.Fake<Infrastructure.Persistence.Repository.IPnsRegistrationRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var fakePushService = A.Fake<IPushService>();

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(identity.Address);

        A.CallTo(() => fakeUserContext.GetDeviceIdOrNull())
            .Returns(randomDeviceId);

        A.CallTo(() => mockPnsRegistrationRepository.FindByDeviceId(randomDeviceId, CancellationToken.None, true))
            .Returns(new PnsRegistration(identity.Address, randomDeviceId, PnsHandle.Parse("handle", PushNotificationPlatform.Fcm).Value, "keyAppId", Environment.Development));

        var handler = new Application.PushNotifications.Commands.UpdateDeviceRegistration.Handler(fakePushService, fakeUserContext);

        // Act
        await handler.Handle(new UpdateDeviceRegistrationCommand()
        {
            Platform = "fcm",
            Handle = "handle",
            AppId = "keyAppId",
        }, CancellationToken.None);

        // Assert
        A.CallTo(() => fakePushService.UpdateRegistration(
                A<IdentityAddress>._,
                A<DeviceId>._,
                A<PnsHandle>._,
                A<string>._,
                A<Environment>._,
                CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Test()
    {

    }
}
