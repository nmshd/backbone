using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.IntegrationEvents;

public class IdentityDeletionProcessStartedIntegrationEventHandlerTests
{
    [Fact]
    public async Task Sends_notification_after_consuming_integration_event()
    {
        // Arrange
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var handler = new IdentityDeletionProcessStartedIntegrationEventHandler(mockPushNotificationSender);
        var identity = TestDataGenerator.CreateIdentity();
        var identityDeletionProcessStartedIntegrationEvent = new IdentityDeletionProcessStartedIntegrationEvent(identity, IdentityDeletionProcess.CreateDeletionProcessAsSupport(identity.Address));

        // Act
        await handler.Handle(identityDeletionProcessStartedIntegrationEvent);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessStartedPushNotification>._, CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }
}
