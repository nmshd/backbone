using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerFromRelationshipDeleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerFromRelationshipDeleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;
public class PeerFromRelationshipDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var peerFromRelationshipDeletedDomainEvent = new PeerFromRelationshipDeletedDomainEvent(identityAddress, "someRelationshipId", "somePeerAddress");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerIdentityDeleted, IdentityAddress.Parse(identityAddress), 1,
            new { peerFromRelationshipDeletedDomainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.PeerIdentityDeleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new PeerFromRelationshipDeletedDomainEventHandler(mockDbContext,
            A.Fake<IEventBus>(),
            A.Fake<ILogger<PeerFromRelationshipDeletedDomainEventHandler>>());

        // Act
        await handler.Handle(peerFromRelationshipDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(identityAddress, ExternalEventType.PeerIdentityDeleted, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var peerFromRelationshipDeletedDomainEvent = new PeerFromRelationshipDeletedDomainEvent(identityAddress, "someRelationshipId", "somePeerAddress");

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipReactivationCompleted, IdentityAddress.Parse(identityAddress), 1,
            new { peerFromRelationshipDeletedDomainEvent.RelationshipId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.PeerIdentityDeleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new PeerFromRelationshipDeletedDomainEventHandler(fakeDbContext,
            mockEventBus,
            A.Fake<ILogger<PeerFromRelationshipDeletedDomainEventHandler>>());

        // Act
        await handler.Handle(peerFromRelationshipDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }
}
