using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications;
using Backbone.Modules.Devices.Domain.Aggregates.PushNotifications.Handles;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.DatawalletModificationCreated;
using Backbone.Modules.Devices.Infrastructure.PushNotifications;
using Backbone.Modules.Devices.Infrastructure.PushNotifications.Connectors;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.PushNotifications.DatawalletModified;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creating_a_Datawallet_modification_from_device_A_sends_a_filtered_notification_to_device_B()
    {
        // Arrange
        var identity = UnitTestTools.Data.TestDataGenerator.CreateRandomIdentityAddress();
        var deviceA = CreateRandomRegistration(identity);
        var deviceB = CreateRandomRegistration(identity);

        var fakeRepository = A.Fake<IPnsRegistrationsRepository>();
        A.CallTo(() => fakeRepository.FindWithAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns([deviceA, deviceB]);

        var mockConnector = A.Fake<IPnsConnector>();
        var fakeConnectorFactory = A.Fake<PnsConnectorFactoryImpl>();
        A.CallTo(fakeConnectorFactory).Where(x => x.Method.Name == "CreateForFirebaseCloudMessaging").WithReturnType<IPnsConnector>().Returns(mockConnector);

        var pushService = new PushService(fakeRepository, fakeConnectorFactory, A.Fake<ILogger<PushService>>());
        var handler = new DatawalletModifiedDomainEventHandler(pushService);
        var domainEvent = new DatawalletModifiedDomainEvent { Identity = identity, ModifiedByDevice = deviceA.DeviceId };

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => mockConnector.Send(A<IEnumerable<PnsRegistration>>._, A<IPushNotification>._)).MustHaveHappenedOnceExactly();
    }

    private static PnsRegistration CreateRandomRegistration(IdentityAddress identity)
    {
        return new PnsRegistration(identity, UnitTestTools.Data.TestDataGenerator.CreateRandomDeviceId(), PnsHandle.Parse(PushNotificationPlatform.Fcm, "someValue").Value, "someAppId",
            PushEnvironment.Development);
    }
}
