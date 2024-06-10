using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeletionCanceled;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeletionCanceled;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
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
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var peerToBeDeletedDomainEvent = new PeerDeletionCanceledDomainEvent(identityAddress, "someRelationshipId", "somePeerAddress");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerDeletionCanceled, IdentityAddress.Parse(identityAddress), 1,
            new { peerToBeDeletedDomainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.PeerDeletionCanceled,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(peerToBeDeletedDomainEvent);

        //Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(identityAddress, ExternalEventType.PeerDeletionCanceled, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var peerToBeDeletedDomainEvent = new PeerDeletionCanceledDomainEvent(identityAddress, "someRelationshipId", "somePeerAddress");

        var fakeDbContext = A.Dummy<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerDeletionCanceled, IdentityAddress.Parse(identityAddress), 1,
            new { peerToBeDeletedDomainEvent.RelationshipId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.PeerDeletionCanceled,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(fakeDbContext, mockEventBus);

        // Act
        await handler.Handle(peerToBeDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }

    private static PeerDeletionCanceledDomainEventHandler CreateHandler(ISynchronizationDbContext fakeDbContext, IEventBus? mockEventBus = null)
    {
        return new PeerDeletionCanceledDomainEventHandler(fakeDbContext,
            mockEventBus ?? A.Dummy<IEventBus>(),
            A.Fake<ILogger<PeerDeletionCanceledDomainEventHandler>>());
    }
}
