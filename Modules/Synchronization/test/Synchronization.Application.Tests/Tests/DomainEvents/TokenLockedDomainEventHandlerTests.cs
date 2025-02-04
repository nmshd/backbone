using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.TokenLocked;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class TokenLockedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Sends_a_push_notification()
    {
        // Arrange
        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var identityAddress = CreateRandomIdentityAddress();
        var handler = new TokenLockedDomainEventHandler(fakeDbContext);
        var domainEvent = new TokenLockedDomainEvent { TokenId = "TOK00000000000000001", CreatedBy = identityAddress };

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => fakeDbContext.CreateExternalEvent(A<TokenLockedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }
}
