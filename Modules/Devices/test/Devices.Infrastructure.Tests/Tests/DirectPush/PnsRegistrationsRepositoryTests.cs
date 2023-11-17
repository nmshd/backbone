using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Tests;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;
using static Backbone.UnitTestTools.Data.TestDataGenerator;
using Environment = Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Environment;

namespace Backbone.Modules.Devices.Infrastructure.Tests.Tests.DirectPush;
public class PnsRegistrationsRepositoryTests
{
    [Fact]
    public async void Update_of_a_registration_that_does_not_exist_yet()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identity = TestDataGenerator.CreateIdentity();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value;

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, CancellationToken.None, true))
           .Returns((PnsRegistration)null).Once();

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        var pnsRegistration = await directPushService.UpdateRegistration(identity.Address, deviceId, pnsHandle, "keyAppId", Environment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository
            .Add(A<PnsRegistration>._, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Update_existing_PnsRegistration_in_repository()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identity = TestDataGenerator.CreateIdentity();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value;
        var appId = "keyAppId";

        var mockPnsRegistration = new PnsRegistration(identity.Address, deviceId, pnsHandle, appId, Environment.Development);

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, CancellationToken.None, true))
           .Returns(mockPnsRegistration);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        var pnsRegistration = await directPushService.UpdateRegistration(identity.Address, deviceId, pnsHandle, appId, Environment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository
            .Update(A<PnsRegistration>._, CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Delete_existing_PnsRegistration()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();
        var identity = TestDataGenerator.CreateIdentity();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value;
        var pnsRegistration = new PnsRegistration(identity.Address, deviceId, pnsHandle, "keyAppId", Environment.Development);

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, CancellationToken.None, true))
           .Returns(pnsRegistration);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        await directPushService.DeleteRegistration(deviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository.Delete(
                A<List<DeviceId>>.That.Matches(e => e.Count == 1), CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Trying_to_delete_non_existing_PnsRegistration_does_nothing()
    {
        // Arrange
        var deviceId = CreateRandomDeviceId();

        var mockPnsRegistrationsRepository = A.Fake<IPnsRegistrationsRepository>();

        A.CallTo(() => mockPnsRegistrationsRepository.FindByDeviceId(deviceId, CancellationToken.None, true))
           .Returns((PnsRegistration)null);

        var directPushService = CreateDirectPushService(mockPnsRegistrationsRepository);

        // Act
        await directPushService.DeleteRegistration(deviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationsRepository.Delete(
                A<List<DeviceId>>.That.Matches(e => e.Count == 1), CancellationToken.None))
            .MustNotHaveHappened();
    }

    private DirectPushService CreateDirectPushService(IPnsRegistrationsRepository pnsRegistrationsRepository)
    {
        var dummyPnsConnectorFactory = A.Dummy<PnsConnectorFactory>();
        var dummyLogger = A.Dummy<ILogger<DirectPushService>>();

        return new DirectPushService(pnsRegistrationsRepository, dummyPnsConnectorFactory, dummyLogger);
    }
}
