using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.ExternalEventCreated;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.ExternalEvents;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.ExternalEventCreated;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.DomainEvents.Incoming;

public class ExternalEventCreatedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Sends_a_push_notification_to_the_owner_of_the_external_event()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();

        var handler = new ExternalEventCreatedDomainEventHandler(mockPushSender);

        var externalEventOwner = CreateRandomIdentityAddress();

        // Act
        await handler.Handle(new ExternalEventCreatedDomainEvent { Owner = externalEventOwner, IsDeliveryBlocked = false });

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(externalEventOwner, A<ExternalEventCreatedPushNotification>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_not_send_a_push_notification_when_delivery_of_the_external_event_is_blocked()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();

        var handler = new ExternalEventCreatedDomainEventHandler(mockPushSender);

        var externalEventOwner = CreateRandomIdentityAddress();

        // Act
        await handler.Handle(new ExternalEventCreatedDomainEvent { Owner = externalEventOwner, IsDeliveryBlocked = true });

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(externalEventOwner, A<ExternalEventCreatedPushNotification>._, A<CancellationToken>._)).MustNotHaveHappened();
    }
}
