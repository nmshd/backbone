using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.NotificationTexts;
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

    private PushService CreateDirectPushService(IPnsRegistrationsRepository pnsRegistrationsRepository, PnsConnectorFactory? pnsConnectorFactory = null,
        IIdentitiesRepository? identitiesRepository = null)
    {
        identitiesRepository ??= A.Dummy<IIdentitiesRepository>();
        pnsConnectorFactory ??= A.Dummy<PnsConnectorFactory>();
        var logger = A.Dummy<ILogger<PushService>>();

        return new PushService(pnsRegistrationsRepository, pnsConnectorFactory, logger, new PushNotificationTextProvider(new PushNotificationResourceManager()), identitiesRepository);
    }
}
