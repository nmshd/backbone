using ExternalEventCreatedSlice = Backbone.Modules.Devices.Module.Features.DomainEvents.ExternalEventCreated;
using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.ExternalEvents;
using Backbone.Modules.Synchronization.Contracts.DomainEvents;
using FakeItEasy;

namespace Backbone.Modules.Devices.Module.Tests.Features.DomainEvents.ExternalEventCreated;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Sends_a_push_notification_to_the_owner_of_the_external_event()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var identity = TestDataGenerator.CreateIdentity();

        var handler = new ExternalEventCreatedSlice.Handler(mockPushSender, fakeIdentitiesRepository);

        var externalEventOwner = CreateRandomIdentityAddress();

        A.CallTo(() => fakeIdentitiesRepository.Get(externalEventOwner, A<CancellationToken>._, A<bool>._)).Returns(identity);

        // Act
        await handler.Handle(new ExternalEventCreatedDomainEvent { EventId = Guid.NewGuid().ToString(), Owner = externalEventOwner, IsDeliveryBlocked = false });

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(
            A<ExternalEventCreatedPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(externalEventOwner)),
            A<CancellationToken>._)
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_not_send_push_notification_if_owner_is_in_status_to_be_deleted()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var identity = TestDataGenerator.CreateIdentity();

        var handler = new ExternalEventCreatedSlice.Handler(mockPushSender, fakeIdentitiesRepository);

        var externalEventOwner = CreateRandomIdentityAddress();

        identity.StartDeletionProcess(identity.Devices[0].Id);

        A.CallTo(() => fakeIdentitiesRepository.Get(externalEventOwner, A<CancellationToken>._, A<bool>._)).Returns(identity);

        // Act
        await handler.Handle(new ExternalEventCreatedDomainEvent { EventId = Guid.NewGuid().ToString(), Owner = externalEventOwner, IsDeliveryBlocked = false });

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(
            A<ExternalEventCreatedPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(externalEventOwner)),
            A<CancellationToken>._)
        ).MustNotHaveHappened();
    }

    [Fact]
    public async Task Does_not_send_a_push_notification_when_delivery_of_the_external_event_is_blocked()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();
        var fakeIdentitiesRepository = A.Fake<IIdentitiesRepository>();
        var identity = TestDataGenerator.CreateIdentity();

        var handler = new ExternalEventCreatedSlice.Handler(mockPushSender, fakeIdentitiesRepository);

        var externalEventOwner = CreateRandomIdentityAddress();

        A.CallTo(() => fakeIdentitiesRepository.Get(externalEventOwner, A<CancellationToken>._, A<bool>._)).Returns(identity);

        // Act
        await handler.Handle(new ExternalEventCreatedDomainEvent { EventId = Guid.NewGuid().ToString(), Owner = externalEventOwner, IsDeliveryBlocked = true });

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(
            A<ExternalEventCreatedPushNotification>._,
            A<SendPushNotificationFilter>.That.Matches(f => f.IncludedIdentities.Contains(externalEventOwner)),
            A<CancellationToken>._)
        ).MustNotHaveHappened();
    }
}
