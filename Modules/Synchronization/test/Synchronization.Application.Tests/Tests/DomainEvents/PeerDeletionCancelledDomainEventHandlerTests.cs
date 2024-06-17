using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCanceled;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCanceled;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class PeerDeletionCancelledDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfIdentityWithDeletionCancelled = TestDataGenerator.CreateRandomIdentityAddress();
        var domainEvent = new PeerDeletionCancelledDomainEvent(peerOfIdentityWithDeletionCancelled, "some-relationship-id", "some-deletedIdentity-id");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerDeletionCanceled, IdentityAddress.Parse(peerOfIdentityWithDeletionCancelled), 1,
            new { domainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            peerOfIdentityWithDeletionCancelled,
            ExternalEventType.PeerDeletionCanceled,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new PeerDeletionCancelledDomainEventHandler(mockDbContext,
            A.Fake<ILogger<PeerDeletionCancelledDomainEventHandler>>());

        // Act
        await handler.Handle(domainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(peerOfIdentityWithDeletionCancelled, ExternalEventType.PeerDeletionCanceled, A<object>._))
            .MustHaveHappenedOnceExactly();
    }
}
