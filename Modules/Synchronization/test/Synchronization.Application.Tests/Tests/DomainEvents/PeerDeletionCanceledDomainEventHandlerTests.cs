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
public class PeerDeletionCanceledDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfIdentityWithDeletionCanceled = TestDataGenerator.CreateRandomIdentityAddress();
        var peerDeletionCanceledDomainEvent = new PeerDeletionCanceledDomainEvent(peerOfIdentityWithDeletionCanceled, "some-relationship-id", "some-deletedIdentity-id");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerDeletionCanceled, IdentityAddress.Parse(peerOfIdentityWithDeletionCanceled), 1,
            new { peerDeletionCanceledDomainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            peerOfIdentityWithDeletionCanceled,
            ExternalEventType.PeerDeletionCanceled,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new PeerDeletionCanceledDomainEventHandler(mockDbContext,
            A.Fake<ILogger<PeerDeletionCanceledDomainEventHandler>>());

        // Act
        await handler.Handle(peerDeletionCanceledDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(peerOfIdentityWithDeletionCanceled, ExternalEventType.PeerDeletionCanceled, A<object>._))
            .MustHaveHappenedOnceExactly();
    }
}
