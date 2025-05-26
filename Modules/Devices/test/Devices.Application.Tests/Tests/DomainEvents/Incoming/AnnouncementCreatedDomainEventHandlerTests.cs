using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.AnnouncementCreated;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.AnnouncementCreated;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.DomainEvents.Incoming;

public class AnnouncementCreatedDomainEventHandlerTests
{
    [Fact]
    public async Task Sends_push_notification_for_announcement()
    {
        // Arrange
        var mockPushSenderService = A.Fake<IPushNotificationSender>();
        var handler = new AnnouncementCreatedDomainEventHandler(mockPushSenderService);

        // Act
        await handler.Handle(new AnnouncementCreatedDomainEvent
        {
            Id = "someAnnouncementId", IsSilent = false, Recipients = [CreateRandomIdentityAddress()], Severity = "High",
            Texts = [new AnnouncementCreatedDomainEventText { Language = "en", Title = "Test Title", Body = "Test Body" }], CreationDate = DateTime.UtcNow, DomainEventId = ""
        });

        // Assert
        A.CallTo(() => mockPushSenderService.SendNotification(A<IPushNotification>._, A<SendPushNotificationFilter>._, A<Dictionary<string, NotificationText>>._, A<CancellationToken>._))
            .MustHaveHappened();
    }

    [Fact]
    public async Task Does_not_send_a_push_notification_for_silent_announcement()
    {
        // Arrange
        var mockPushSenderService = A.Fake<IPushNotificationSender>();
        var handler = new AnnouncementCreatedDomainEventHandler(mockPushSenderService);

        // Act
        await handler.Handle(new AnnouncementCreatedDomainEvent
        {
            Id = "someAnnouncementId", IsSilent = true, Recipients = [CreateRandomIdentityAddress()], Severity = "High",
            Texts = [new AnnouncementCreatedDomainEventText { Language = "en", Title = "Test Title", Body = "Test Body" }], CreationDate = DateTime.UtcNow, DomainEventId = ""
        });

        // Assert
        A.CallTo(() => mockPushSenderService.SendNotification(A<IPushNotification>._, A<SendPushNotificationFilter>._, A<Dictionary<string, NotificationText>>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
