﻿using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipStatusChanged;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

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
            Peer = relationshipTo,
            NewStatus = "Pending",
            Initiator = TestDataGenerator.CreateRandomIdentityAddress()
        };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(@event);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<RelationshipStatusChangedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_not_create_an_external_event_if_new_status_is_ReadyForDeletion()
    {
        // Arrange
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipStatusChangedDomainEvent
        {
            RelationshipId = "REL1",
            Peer = relationshipTo,
            NewStatus = "ReadyForDeletion",
            Initiator = TestDataGenerator.CreateRandomIdentityAddress()
        };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(@event);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<RelationshipStatusChangedExternalEvent>._)).MustNotHaveHappened();
    }

    [Theory]
    [InlineData("DeletionProposed")]
    [InlineData("ReadyForDeletion")]
    public async Task Calls_DeleteUnsyncedExternalEventsWithOwnerAndContext_when_new_status_is_DeletionProposed_or_ReadyForDeletion(string newStatus)
    {
        // Arrange
        var relationshipTo = TestDataGenerator.CreateRandomIdentityAddress();
        const string relationshipId = "REL11111111111111111";
        var initiator = TestDataGenerator.CreateRandomIdentityAddress();
        var @event = new RelationshipStatusChangedDomainEvent
        {
            RelationshipId = relationshipId,
            Peer = relationshipTo,
            NewStatus = newStatus,
            Initiator = initiator
        };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(@event);

        // Assert
        A.CallTo(() => mockDbContext.DeleteUnsyncedExternalEventsWithOwnerAndContext(initiator, relationshipId)).MustHaveHappenedOnceExactly();
    }

    private static RelationshipStatusChangedDomainEventHandler CreateHandler(ISynchronizationDbContext dbContext)
    {
        return new RelationshipStatusChangedDomainEventHandler(dbContext, A.Fake<ILogger<RelationshipStatusChangedDomainEventHandler>>());
    }
}
