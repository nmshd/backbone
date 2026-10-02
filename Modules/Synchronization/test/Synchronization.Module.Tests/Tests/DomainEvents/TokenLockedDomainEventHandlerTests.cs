using TokenLocked = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.TokenLocked;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Tokens.Contracts.DomainEvents;
using FakeItEasy;

namespace Backbone.Modules.Synchronization.Module.Tests.Tests.DomainEvents;

public class TokenLockedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Sends_a_push_notification()
    {
        // Arrange
        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var identityAddress = CreateRandomIdentityAddress();
        var handler = new TokenLocked.Handler(fakeDbContext);
        var domainEvent = new TokenLockedDomainEvent { TokenId = "TOK00000000000000001", CreatedBy = identityAddress };

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => fakeDbContext.CreateExternalEvent(A<TokenLockedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }
}
