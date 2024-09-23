﻿using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerToBeDeleted;
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
        var peerOfIdentityToBeDeleted = TestDataGenerator.CreateRandomIdentityAddress();

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var gracePeriodEndsAt = DateTime.Parse("2015-07-23");

        var handler = CreateHandler(mockDbContext);

        // Act
        await handler.Handle(new PeerToBeDeletedDomainEvent(peerOfIdentityToBeDeleted, "some-relationship-id", "some-deletedIdentity-id", gracePeriodEndsAt));

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(
                peerOfIdentityToBeDeleted,
                ExternalEventType.PeerToBeDeleted,
                A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    private static PeerToBeDeletedDomainEventHandler CreateHandler(ISynchronizationDbContext mockDbContext)
    {
        return new PeerToBeDeletedDomainEventHandler(mockDbContext, A.Fake<ILogger<PeerToBeDeletedDomainEventHandler>>());
    }
}
