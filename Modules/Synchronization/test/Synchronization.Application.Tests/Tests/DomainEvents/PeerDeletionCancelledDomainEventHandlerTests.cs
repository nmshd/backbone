using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCancelled;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class PeerDeletionCancelledDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfIdentityWithDeletionCancelled = CreateRandomIdentityAddress();
            <<<<<<< HEAD
        var domainEvent = new PeerDeletionCancelledDomainEvent
                { PeerOfIdentityWithDeletionCancelled = peerOfIdentityWithDeletionCancelled, RelationshipId = "some-relationship-id", IdentityWithDeletionCancelled = "some-deletedIdentity-id" };
            ====== =
        var domainEvent = new PeerDeletionCancelledDomainEvent(peerOfIdentityWithDeletionCancelled, "some-relationship-id", "some-deletedIdentity-id");
            >>>>>>> parent of 956b033b(WIP: required properties in events)

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new PeerDeletionCancelledDomainEventHandler(mockDbContext,
            A.Fake<ILogger<PeerDeletionCancelledDomainEventHandler>>());

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<PeerDeletionCancelledExternalEvent>._))
            .MustHaveHappenedOnceExactly();
    }
}
