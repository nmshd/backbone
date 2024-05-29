﻿using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class RelationshipStatusChangedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipStatusChangedDomainEvent
        {
            RelationshipId = "REL1",
            Peer = relationshipTo
        };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipStatusChanged, relationshipTo, 1,
            new { @event.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            relationshipTo,
            ExternalEventType.RelationshipStatusChanged,
            A<object>._)
        ).Returns(externalEvent);

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(@event);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(relationshipTo, ExternalEventType.RelationshipStatusChanged, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_an_ExternalEventCreatedDomainEvent()
    {
        // Arrange
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipStatusChangedDomainEvent
        {
            RelationshipId = "REL1",
            Peer = relationshipTo
        };

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStarted, IdentityAddress.Parse(relationshipTo), 1,
            new { @event.RelationshipId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            relationshipTo,
            ExternalEventType.RelationshipStatusChanged,
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

    private RelationshipStatusChangedDomainEventHandler CreateHandler(ISynchronizationDbContext dbContext, IEventBus? eventBus = null)
    {
        eventBus ??= A.Fake<IEventBus>();
        return new RelationshipStatusChangedDomainEventHandler(dbContext, eventBus, A.Fake<ILogger<RelationshipStatusChangedDomainEventHandler>>());
    }
}
