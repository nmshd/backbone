using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class IdentityDeletionProcessStartedDomainEventHandlerTests
{
    [Fact]
    public async Task Creates_an_external_event_if_initiator_is_someone_else()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent(identityAddress, "some-deletion-process-id", null);

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStarted, IdentityAddress.Parse(identityAddress), 1,
            new { identityDeletionProcessStartedDomainEvent.DeletionProcessId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            A<IdentityAddress>.That.Matches(i => i.Value == identityAddress),
            ExternalEventType.IdentityDeletionProcessStarted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStartedDomainEventHandler(fakeDbContext, A.Fake<ILogger<IdentityDeletionProcessStartedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Handle
        A.CallTo(() => fakeDbContext
            .CreateExternalEvent(identityAddress, ExternalEventType.IdentityDeletionProcessStarted, A<object>._)
        ).MustHaveHappenedOnceExactly();

        externalEvent.DomainEvents.Should().HaveCount(1);
        externalEvent.DomainEvents[0].Should().BeOfType<ExternalEventCreatedDomainEvent>();

        var createdDomainEvent = (ExternalEventCreatedDomainEvent)externalEvent.DomainEvents[0];
        createdDomainEvent.EventId.Should().Be(externalEvent.Id.Value);
        createdDomainEvent.Owner.Should().Be(externalEvent.Owner.Value);
    }

    [Fact]
    public async Task Does_nothing_if_initiator_is_deletion_process_owner()
    {
        // Arrange
        var deletionProcessOwner = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent(deletionProcessOwner, "some-deletion-process-id", deletionProcessOwner);

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStarted, IdentityAddress.Parse(deletionProcessOwner), 1,
            new { identityDeletionProcessStartedDomainEvent.DeletionProcessId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            A<IdentityAddress>.That.Matches(i => i.Value == deletionProcessOwner),
            ExternalEventType.IdentityDeletionProcessStarted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStartedDomainEventHandler(fakeDbContext, mockEventBus, A.Fake<ILogger<IdentityDeletionProcessStartedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Assert
        A.CallTo(() => mockEventBus.Publish(A<ExternalEventCreatedDomainEvent>._)).MustNotHaveHappened();
    }
}
