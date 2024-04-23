using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class RelationshipCreatedDomainEventHandlerTests
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var relationshipFrom = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipCreatedDomainEvent
        {
            RelationshipId = "REL1",
            From = relationshipFrom,
            To = relationshipTo
        };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipCreated, relationshipTo, 1,
            new { @event.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            relationshipTo,
            ExternalEventType.RelationshipCreated,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(@event);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(relationshipTo, ExternalEventType.RelationshipCreated, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_an_ExternalEventCreatedDomainEvent()
    {
        // Arrange
        var relationshipFrom = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipCreatedDomainEvent
        {
            RelationshipId = "REL1",
            From = relationshipFrom,
            To = relationshipTo
        };

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStarted, IdentityAddress.Parse(relationshipTo), 1,
            new { @event.RelationshipId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            relationshipTo,
            ExternalEventType.RelationshipCreated,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(fakeDbContext, mockEventBus);

        // Act
        await handler.Handle(@event);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == relationshipTo))
        ).MustHaveHappenedOnceExactly();
    }

    private RelationshipCreatedDomainEventHandler CreateHandler(ISynchronizationDbContext dbContext, IEventBus? eventBus = null)
    {
        eventBus ??= A.Fake<IEventBus>();
        return new RelationshipCreatedDomainEventHandler(dbContext, eventBus, A.Fake<ILogger<RelationshipCreatedDomainEventHandler>>());
    }
}
