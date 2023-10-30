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
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

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
        var directPushIdentifier = DevicePushIdentifier.Create(randomDeviceId);

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

        A.CallTo(() => mockPnsRegistrationRepository.Update(
                A<PnsRegistration>._,
                CancellationToken.None))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task Updating_an_existing_PNS_registration()
    {
        // Arrange
        var randomDeviceId = TestDataGenerator.CreateRandomDeviceId();
        var identity = TestDataGenerator.CreateIdentity();

        var mockPnsRegistrationRepository = A.Fake<Infrastructure.Persistence.Repository.IPnsRegistrationRepository>();
        var fakeUserContext = A.Fake<IUserContext>();
        var fakePushService = A.Fake<IPushService>();

        A.CallTo(() => fakeUserContext.GetAddressOrNull())
            .Returns(identity.Address);

        A.CallTo(() => fakeUserContext.GetDeviceIdOrNull())
            .Returns(randomDeviceId);

        A.CallTo(() => mockPnsRegistrationRepository.FindByDeviceId(randomDeviceId, CancellationToken.None, true))
            .Returns((PnsRegistration)null);

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

        A.CallTo(() => mockPnsRegistrationRepository.Add(
                A<PnsRegistration>._,
                CancellationToken.None))
            .Returns(Task.CompletedTask);
    }
}
