using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.PeerDeleted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.PeerDeleted;
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
        var peerOfDeletedIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var peerDeletedDomainEvent = new PeerDeletedDomainEvent(peerOfDeletedIdentity, "some-relationship-id", "some-deletedIdentity-id");

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.PeerDeleted, IdentityAddress.Parse(peerOfDeletedIdentity), 1,
            new { peerDeletedDomainEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            peerOfDeletedIdentity,
            ExternalEventType.PeerDeleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new PeerDeletedDomainEventHandler(mockDbContext,
            A.Fake<ILogger<PeerDeletedDomainEventHandler>>());

        // Act
        await handler.Handle(peerDeletedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(peerOfDeletedIdentity, ExternalEventType.PeerDeleted, A<object>._))
            .MustHaveHappenedOnceExactly();
    }
}
