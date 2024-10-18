using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.PushNotifications;

public class PushServiceTests : AbstractTestsBase
{
    [Fact]
    public async Task Update_of_a_registration_that_does_not_exist_yet()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identityAddress = CreateRandomIdentityAddress();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value;

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, A<CancellationToken>._, A<bool>._))
            .Returns<PnsRegistration?>(null).Once();

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        var devicePushIdentifier = await directPushService.UpdateRegistration(identityAddress, deviceId, pnsHandle, "someAppId", PushEnvironment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository
                .Add(A<PnsRegistration>.That.Matches(p => p.DevicePushIdentifier == devicePushIdentifier), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Update_existing_PnsRegistration_in_repository()
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
    public async Task Delete_existing_PnsRegistration()
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
    public async Task Trying_to_delete_non_existing_PnsRegistration_does_nothing()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, A<CancellationToken>._, A<bool>._))
            .Returns<PnsRegistration?>(null);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        await directPushService.DeleteRegistration(deviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository.Delete(
                A<List<DeviceId>>._, CancellationToken.None))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task SendFilteredNotification_does_not_call_connectors_for_filtered_devices()
    {
        // Arrange
        var filteredDeviceId = CreateRandomDeviceId();
        var otherDeviceId = CreateRandomDeviceId();
        var address = CreateRandomIdentityAddress();

        var fakePnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();
        var mockConnector = A.Fake<IPnsConnector>();

        var registrationToFilter = CreatePnsRegistration(address, filteredDeviceId);
        var otherRegistration = CreatePnsRegistration(address, otherDeviceId);

        A.CallTo(() => fakePnsRegistrationsRepository.FindWithAddress(address, A<CancellationToken>._, A<bool>._))!
            .Returns<IEnumerable<PnsRegistration>?>(new List<PnsRegistration>
            {
                registrationToFilter,
                otherRegistration
            });

        var directPushService = CreateDirectPushService(fakePnsRegistrationsRepository, FakePnsConnectorFactory.Returning(mockConnector));

        // Act
        await directPushService.SendFilteredNotification(address, new TestPushNotification(), [filteredDeviceId], CancellationToken.None);

        // Assert
        A.CallTo(() => mockConnector.Send(
                A<IEnumerable<PnsRegistration>>.That.Matches(r => r.Count() == 1 && r.First() == otherRegistration),
                A<TestPushNotification>._))
            .MustHaveHappenedOnceExactly();
    }

    private static PnsRegistration CreatePnsRegistration(IdentityAddress address, DeviceId filteredDeviceId)
    {
        return new PnsRegistration(address, filteredDeviceId, PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value, "someAppId", PushEnvironment.Development);
    }

    private PushService CreateDirectPushService(IPnsRegistrationsRepository pnsRegistrationsRepository, PnsConnectorFactory? pnsConnectorFactory = null)
    {
        pnsConnectorFactory ??= A.Dummy<PnsConnectorFactory>();
        var logger = A.Dummy<ILogger<PushService>>();

        return new PushService(pnsRegistrationsRepository, pnsConnectorFactory, logger);
    }

    private class FakePnsConnectorFactory : PnsConnectorFactory
    {
        private readonly IPnsConnector _connector;

        private FakePnsConnectorFactory(IPnsConnector connector)
        {
            _connector = connector;
        }

        protected override IPnsConnector CreateForFirebaseCloudMessaging()
        {
            return _connector;
        }

        protected override IPnsConnector CreateForApplePushNotificationService()
        {
            return _connector;
        }

        protected override IPnsConnector CreateForDummy()
        {
            return _connector;
        }

        protected override IPnsConnector CreateForSse()
        {
            return _connector;
        }

        public static PnsConnectorFactory Returning(IPnsConnector connector)
        {
            return new FakePnsConnectorFactory(connector);
        }
    }
}
