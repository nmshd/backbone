using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerToBeDeleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;
public class PeerToBeDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var peerOfToBeDeletedIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var peerToBeDeletedDomainEvent = new PeerToBeDeletedDomainEvent(peerOfToBeDeletedIdentity, "some-relationship-id", "some-deletedIdentity-id", "2024-08-25 16:13:34.596561+00");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerToBeDeleted, IdentityAddress.Parse(peerOfToBeDeletedIdentity), 1,
            new { peerToBeDeletedDomainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            peerOfToBeDeletedIdentity,
            ExternalEventType.PeerToBeDeleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new PeerToBeDeletedDomainEventHandler(mockDbContext,
            A.Fake<ILogger<PeerToBeDeletedDomainEventHandler>>());

        // Act
        await handler.Handle(peerToBeDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(peerOfToBeDeletedIdentity, ExternalEventType.PeerToBeDeleted, A<object>._))
            .MustHaveHappenedOnceExactly();
    }
}
