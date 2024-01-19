using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;
public class DirectPushServiceTests
{
    [Fact]
    public async void Update_of_a_registration_that_does_not_exist_yet()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identityAddress = CreateRandomIdentityAddress();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value;

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, A<CancellationToken>._, A<bool>._))
           .Returns((PnsRegistration?)null).Once();

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        var devicePushIdentifier = await directPushService.UpdateRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository
            .Add(A<PnsRegistration>.That.Matches(p => p.DevicePushIdentifier == devicePushIdentifier), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Update_existing_PnsRegistration_in_repository()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identityAddress = CreateRandomIdentityAddress();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value;

        var pnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development);

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, A<CancellationToken>._, A<bool>._))
           .Returns(pnsRegistration);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        var devicePushIdentifier = await directPushService.UpdateRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository
            .Update(A<PnsRegistration>.That.Matches(p => p.DevicePushIdentifier == devicePushIdentifier), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Delete_existing_PnsRegistration()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identityAddress = CreateRandomIdentityAddress();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value;
        var pnsRegistration = new PnsRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development);

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, A<CancellationToken>._, A<bool>._))
           .Returns(pnsRegistration);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        await directPushService.DeleteRegistration(deviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository.Delete(
                A<List<DeviceId>>.That.Matches(e => e.FirstOrDefault() == deviceId), CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Trying_to_delete_non_existing_PnsRegistration_does_nothing()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, A<CancellationToken>._, A<bool>._))
           .Returns((PnsRegistration?)null);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        await directPushService.DeleteRegistration(deviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository.Delete(
                A<List<DeviceId>>._, CancellationToken.None))
            .MustNotHaveHappened();
    }

    private DirectPushService CreateDirectPushService(IPnsRegistrationsRepository pnsRegistrationsRepository)
    {
        var dummyPnsConnectorFactory = A.Dummy<PnsConnectorFactory>();
        var dummyLogger = A.Dummy<ILogger<DirectPushService>>();

        return new DirectPushService(pnsRegistrationsRepository, dummyPnsConnectorFactory, dummyLogger);
    }
}
