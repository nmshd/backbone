using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Application.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Application.Infrastructure.PushNotifications.Tokens;
using Backbone.Modules.Devices.Domain.DomainEvents.Incoming.TokenLocked;
using FakeItEasy;

namespace Backbone.Modules.Devices.Application.Tests.Tests.DomainEvents.Incoming;

public class TokenLockedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Sends_a_push_notification()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();
        var fakeRepository = A.Fake<IIdentitiesRepository>();
        var identity = TestDataGenerator.CreateIdentity();

        A.CallTo(() => fakeRepository.FindByAddress(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var handler = new TokenLockedDomainEventHandler(mockPushSender, fakeRepository);
        var domainEvent = new TokenLockedDomainEvent { TokenId = "TOK00000000000000001", CreatedBy = identity.Address };

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(A<TokenLockedPushNotification>._, A<SendPushNotificationFilter>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
