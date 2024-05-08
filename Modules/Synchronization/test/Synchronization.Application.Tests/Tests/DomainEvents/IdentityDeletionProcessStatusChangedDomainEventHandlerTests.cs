using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStatusChanged;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class IdentityDeletionProcessStatusChangedDomainEventHandlerTests
{
    [Fact]
    public async Task Creates_an_external_event_if_initiator_is_someone_else()
    {
        // Arrange
        var deletionProcessOwner = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStatusChangedDomainEvent = new IdentityDeletionProcessStatusChangedDomainEvent(deletionProcessOwner, "someDeletionProcessId", null);

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStatusChanged, IdentityAddress.Parse(deletionProcessOwner), 1,
            new { identityDeletionProcessStatusChangedDomainEvent.DeletionProcessId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            deletionProcessOwner,
            ExternalEventType.IdentityDeletionProcessStatusChanged,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStatusChangedDomainEventHandler(mockDbContext,
            A.Fake<IEventBus>(),
            A.Fake<ILogger<IdentityDeletionProcessStatusChangedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStatusChangedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(deletionProcessOwner, ExternalEventType.IdentityDeletionProcessStatusChanged, A<object>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Publishes_a_domain_event_after_creating_an_external_event()
    {
        // Arrange
        var deletionProcessOwner = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStatusChangedDomainEvent = new IdentityDeletionProcessStatusChangedDomainEvent(deletionProcessOwner, "someDeletionProcessId", null);

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStatusChanged, IdentityAddress.Parse(deletionProcessOwner), 1,
            new { identityDeletionProcessStatusChangedDomainEvent.DeletionProcessId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            deletionProcessOwner,
            ExternalEventType.IdentityDeletionProcessStatusChanged,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStatusChangedDomainEventHandler(fakeDbContext,
            mockEventBus,
            A.Fake<ILogger<IdentityDeletionProcessStatusChangedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStatusChangedDomainEvent);

        // Assert
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_nothing_if_initiator_is_deletion_process_owner()
    {
        // Arrange
        var deletionProcessOwner = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStatusChangedDomainEvent = new IdentityDeletionProcessStatusChangedDomainEvent(deletionProcessOwner, "someDeletionProcessId", deletionProcessOwner);

        var mockDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStatusChanged, IdentityAddress.Parse(deletionProcessOwner), 1,
            new { identityDeletionProcessStatusChangedDomainEvent.DeletionProcessId });

        A.CallTo(() => mockDbContext.CreateExternalEvent(
            deletionProcessOwner,
            ExternalEventType.IdentityDeletionProcessStatusChanged,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStatusChangedDomainEventHandler(mockDbContext,
            mockEventBus,
            A.Fake<ILogger<IdentityDeletionProcessStatusChangedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStatusChangedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<IdentityAddress>._, A<ExternalEventType>._, A<object>._)).MustNotHaveHappened();
        A.CallTo(() => mockEventBus.Publish(A<IdentityDeletionProcessStatusChangedDomainEvent>._)).MustNotHaveHappened();
    }
}
