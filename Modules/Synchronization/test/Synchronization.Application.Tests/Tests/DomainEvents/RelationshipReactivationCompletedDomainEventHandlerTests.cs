using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.RelationshipReactivationCompleted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;
public class RelationshipReactivationCompletedDomainEventHandlerTests
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipReactivationCompletedIntegrationEvent = new RelationshipReactivationCompletedDomainEvent("someRelationshipId", identityAddress);

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipReactivationCompleted, IdentityAddress.Parse(identityAddress), 1,
            new { relationshipReactivationCompletedIntegrationEvent.RelationshipId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.RelationshipReactivationCompleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new RelationshipReactivationCompletedDomainEventHandler(mockDbContext,
            A.Fake<IEventBus>(),
            A.Fake<ILogger<RelationshipReactivationCompletedDomainEventHandler>>());

        // Act
        await handler.Handle(relationshipReactivationCompletedIntegrationEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(identityAddress, ExternalEventType.RelationshipReactivationCompleted, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var relationshipReactivationCompletedIntegrationEvent = new RelationshipReactivationCompletedDomainEvent("someRelationshipId", identityAddress);

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.RelationshipReactivationCompleted, IdentityAddress.Parse(identityAddress), 1,
            new { relationshipReactivationCompletedIntegrationEvent.RelationshipId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            identityAddress,
            ExternalEventType.RelationshipReactivationCompleted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new RelationshipReactivationCompletedDomainEventHandler(fakeDbContext,
            mockEventBus,
            A.Fake<ILogger<RelationshipReactivationCompletedDomainEventHandler>>());

        // Act
        await handler.Handle(relationshipReactivationCompletedIntegrationEvent);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }
}
