using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.DirectPush;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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

    [Fact]
    public async void Updating_existing_PnsRegistration_in_repository()
    {
        // Arrange
        var randomDeviceId = CreateRandomDeviceId();
        var randomIdentity = TestDataGenerator.CreateIdentity();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value;
        var appId = "keyAppId";

        var mockPnsRegistration = new PnsRegistration(randomIdentity.Address, randomDeviceId, pnsHandle, appId, Environment.Development);

        var mockPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();
        var dummyPnsConnectorFactory = A.Fake<PnsConnectorFactory>();
        var dummyLogger = A.Fake<ILogger<DirectPushService>>();
        var dummyPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();

        A.CallTo(() => mockPnsRegistrationRepository.FindByDeviceId(randomDeviceId, CancellationToken.None, true))
           .Returns(mockPnsRegistration);

        var directPushService = new DirectPushService(mockPnsRegistrationRepository, dummyPnsConnectorFactory, dummyLogger, dummyPnsRegistrationRepository);

        // Act
        var pnsRegistration = await directPushService.UpdateRegistration(randomIdentity.Address, randomDeviceId, pnsHandle, appId, Environment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationRepository
            .Update(A<PnsRegistration>._, CancellationToken.None))
            .MustHaveHappenedOnceExactly();

        pnsRegistration.Should().BeOfType<DevicePushIdentifier>();
    }

    [Fact]
    public async void Adding_new_PnsRegistration_in_repository()
    {
        // Arrange
        var randomDeviceId = CreateRandomDeviceId();
        var randomIdentity = TestDataGenerator.CreateIdentity();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value;
        var appId = "keyAppId";

        var mockPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();
        var mockPnsRegistrationRepository1 = A.Fake<IPnsRegistrationRepository>();
        var dummyPnsConnectorFactory = A.Fake<PnsConnectorFactory>();
        var dummyLogger = A.Fake<ILogger<DirectPushService>>();
        var dummyPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();

        A.CallTo(() => mockPnsRegistrationRepository.FindByDeviceId(randomDeviceId, CancellationToken.None, true))
           .Returns((PnsRegistration)null).Once();

        var directPushService = new DirectPushService(mockPnsRegistrationRepository, dummyPnsConnectorFactory, dummyLogger, dummyPnsRegistrationRepository);

        // Act
        var pnsRegistration = await directPushService.UpdateRegistration(randomIdentity.Address, randomDeviceId, pnsHandle, appId, Environment.Development, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationRepository
            .Add(A<PnsRegistration>._, CancellationToken.None))
            .MustHaveHappenedOnceExactly();

        pnsRegistration.Should().BeOfType<DevicePushIdentifier>();
    }

    [Fact]
    public async void Trying_to_delete_existing_PnsRegistration()
    {
        // Arrange
        var randomDeviceId = CreateRandomDeviceId();
        var randomIdentity = TestDataGenerator.CreateIdentity();

        var pnsHandle = PnsHandle.Parse(PushNotificationPlatform.Fcm, "handle").Value;
        var appId = "keyAppId";
        var pnsRegistration = new PnsRegistration(randomIdentity.Address, randomDeviceId, pnsHandle, appId, Environment.Development);

        var mockPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();
        var dummyPnsConnectorFactory = A.Fake<PnsConnectorFactory>();
        var dummyLogger = A.Fake<ILogger<DirectPushService>>();
        var dummyPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();

        A.CallTo(() => mockPnsRegistrationRepository.FindByDeviceId(randomDeviceId, CancellationToken.None, true))
           .Returns(pnsRegistration);

        var directPushService = new DirectPushService(mockPnsRegistrationRepository, dummyPnsConnectorFactory, dummyLogger, dummyPnsRegistrationRepository);

        // Act
        await directPushService.DeleteRegistration(randomDeviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationRepository.Delete(
                A<List<DeviceId>>.That.Matches(e => e.Count == 1), CancellationToken.None))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Trying_to_delete_non_existing_PnsRegistration()
    {
        // Arrange
        var randomDeviceId = CreateRandomDeviceId();
        var randomIdentity = TestDataGenerator.CreateIdentity();

        var mockPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();
        var dummyPnsConnectorFactory = A.Fake<PnsConnectorFactory>();
        var dummyLogger = A.Fake<ILogger<DirectPushService>>();
        var dummyPnsRegistrationRepository = A.Fake<IPnsRegistrationRepository>();

        A.CallTo(() => mockPnsRegistrationRepository.FindByDeviceId(randomDeviceId, CancellationToken.None, true))
           .Returns((PnsRegistration)null);

        var directPushService = new DirectPushService(mockPnsRegistrationRepository, dummyPnsConnectorFactory, dummyLogger, dummyPnsRegistrationRepository);

        // Act
        await directPushService.DeleteRegistration(randomDeviceId, CancellationToken.None);

        // Assert
        A.CallTo(() => mockPnsRegistrationRepository.Delete(
                A<List<DeviceId>>.That.Matches(e => e.Count == 1), CancellationToken.None))
            .MustNotHaveHappened();
    }
}
