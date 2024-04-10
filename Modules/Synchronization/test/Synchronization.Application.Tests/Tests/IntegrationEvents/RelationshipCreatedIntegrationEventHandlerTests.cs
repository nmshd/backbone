using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.RelationshipCreated;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.IntegrationEvents;

public class RelationshipCreatedIntegrationEventHandlerTests
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var relationshipFrom = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipCreatedIntegrationEvent
        {
            RelationshipId = "REL1",
            From = relationshipFrom,
            To = relationshipTo
        };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        // ReSharper disable once RedundantAnonymousTypePropertyName
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
    public async Task Publishes_an_ExternalEventCreatedIntegrationEvent()
    {
        // Arrange
        var relationshipFrom = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipCreatedIntegrationEvent
        {
            RelationshipId = "REL1",
            From = relationshipFrom,
            To = relationshipTo
        };

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        // ReSharper disable once RedundantAnonymousTypePropertyName
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
            A<ExternalEventCreatedIntegrationEvent>.That.Matches(e => e.Owner == relationshipTo))
        ).MustHaveHappenedOnceExactly();
    }

    private RelationshipCreatedIntegrationEventHandler CreateHandler(ISynchronizationDbContext dbContext, IEventBus? eventBus = null)
    {
        eventBus ??= A.Fake<IEventBus>();
        return new RelationshipCreatedIntegrationEventHandler(dbContext, eventBus, A.Fake<ILogger<RelationshipCreatedIntegrationEventHandler>>());
    }
}
