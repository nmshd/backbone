using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class PeerDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfDeletedIdentity = CreateRandomIdentityAddress();

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(new PeerDeletedDomainEvent { PeerOfDeletedIdentity = peerOfDeletedIdentity, RelationshipId = "some-relationship-id", DeletedIdentity = "some-deletedIdentity-id" });

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<PeerDeletedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }

    private static PeerDeletedDomainEventHandler CreateHandler(ISynchronizationDbContext mockDbContext)
    {
        return new PeerDeletedDomainEventHandler(mockDbContext, A.Fake<ILogger<PeerDeletedDomainEventHandler>>());
    }
}
