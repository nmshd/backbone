using Backbone.BuildingBlocks.Application.PushNotifications;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Devices.Module.Features.DomainEvents.TokenLocked;
using Backbone.Modules.Devices.Abstractions;
using Backbone.Modules.Devices.Module.Features.PushNotifications.Shared.Tokens;
using Backbone.Modules.Tokens.Contracts.DomainEvents;
using FakeItEasy;

namespace Backbone.Modules.Devices.Module.Tests.Tests.DomainEvents.Incoming;

public class TokenLockedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Sends_a_push_notification()
    {
        // Arrange
        var mockPushSender = A.Fake<IPushNotificationSender>();
        var fakeRepository = A.Fake<IIdentitiesRepository>();
        var identity = TestDataGenerator.CreateIdentity();

        A.CallTo(() => fakeRepository.Get(A<IdentityAddress>._, A<CancellationToken>._, A<bool>._)).Returns(identity);

        var handler = new TokenLockedDomainEventHandler(mockPushSender, fakeRepository);
        var domainEvent = new TokenLockedDomainEvent { TokenId = "TOK00000000000000001", CreatedBy = identity.Address };

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => mockPushSender.SendNotification(A<TokenLockedPushNotification>._, A<SendPushNotificationFilter>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
