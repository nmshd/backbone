using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.DeletionProcess;
using Backbone.Modules.Devices.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using FakeItEasy;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.DomainEvents;

public class IdentityDeletionProcessStartedDomainEventHandlerTests
{
    [Fact]
    public async Task Sends_push_notification()
    {
        // Arrange
        var mockPushNotificationSender = A.Fake<IPushNotificationSender>();
        var handler = new IdentityDeletionProcessStartedDomainEventHandler(mockPushNotificationSender);
        var identity = TestDataGenerator.CreateIdentity();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent(identity.Address, IdentityDeletionProcess.StartAsSupport(identity.Address).Id);

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Assert
        A.CallTo(() => mockPushNotificationSender.SendNotification(identity.Address,
            A<DeletionProcessStartedPushNotification>._, CancellationToken.None)
        ).MustHaveHappenedOnceExactly();
    }
}
