using PeerToBeDeletedSlice = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.PeerToBeDeleted;
using Backbone.Modules.Relationships.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.ExternalEvents.PeerToBeDeleted;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfIdentityToBeDeleted = CreateRandomIdentityAddress();

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var gracePeriodEndsAt = DateTime.Parse("2015-07-23");

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(new PeerToBeDeletedDomainEvent
        { PeerOfIdentityToBeDeleted = peerOfIdentityToBeDeleted, RelationshipId = "some-relationship-id", IdentityToBeDeleted = "some-deletedIdentity-id", GracePeriodEndsAt = gracePeriodEndsAt });

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<PeerToBeDeletedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }

    private static PeerToBeDeletedSlice.Handler CreateHandler(ISynchronizationDbContext mockDbContext)
    {
        return new PeerToBeDeletedSlice.Handler(mockDbContext, A.Fake<ILogger<PeerToBeDeletedSlice.Handler>>());
    }
}
