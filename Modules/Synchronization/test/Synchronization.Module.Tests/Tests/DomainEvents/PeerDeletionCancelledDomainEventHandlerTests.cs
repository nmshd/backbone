using PeerDeletionCancelled = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerDeletionCancelled;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Tests.Tests.DomainEvents;

public class PeerDeletionCancelledDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfIdentityWithDeletionCancelled = CreateRandomIdentityAddress();
        var domainEvent = new PeerDeletionCancelledDomainEvent
        { PeerOfIdentityWithDeletionCancelled = peerOfIdentityWithDeletionCancelled, RelationshipId = "some-relationship-id", IdentityWithDeletionCancelled = "some-deletedIdentity-id" };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new PeerDeletionCancelled.Handler(mockDbContext,
            A.Fake<ILogger<PeerDeletionCancelled.Handler>>());

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<PeerDeletionCancelledExternalEvent>._))
            .MustHaveHappenedOnceExactly();
    }
}
