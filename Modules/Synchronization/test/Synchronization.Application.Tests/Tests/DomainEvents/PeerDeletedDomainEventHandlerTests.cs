using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;
public class PeerDeletedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var peerFromRelationshipDeletedDomainEvent = new PeerDeletedDomainEvent(identityAddress, "someRelationshipId", "somePeerAddress");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerDeleted, IdentityAddress.Parse(identityAddress), 1,
            new { peerFromRelationshipDeletedDomainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.PeerDeleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(peerFromRelationshipDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(identityAddress, ExternalEventType.PeerDeleted, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var peerFromRelationshipDeletedDomainEvent = new PeerDeletedDomainEvent(identityAddress, "someRelationshipId", "somePeerAddress");

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipReactivationCompleted, IdentityAddress.Parse(identityAddress), 1,
            new { peerFromRelationshipDeletedDomainEvent.RelationshipId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.PeerDeleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(fakeDbContext, mockEventBus);

        // Act
        await handler.Handle(peerFromRelationshipDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }

    private static PeerDeletedDomainEventHandler CreateHandler(ISynchronizationDbContext dbContext, IEventBus? eventBus = null)
    {
        return new PeerDeletedDomainEventHandler(dbContext,
            eventBus ?? A.Dummy<IEventBus>(),
            A.Fake<ILogger<PeerDeletedDomainEventHandler>>());
    }
}
