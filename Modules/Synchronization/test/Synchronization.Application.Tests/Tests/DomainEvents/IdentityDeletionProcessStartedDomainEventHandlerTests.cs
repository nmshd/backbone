using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class IdentityDeletionProcessStartedDomainEventHandlerTests
{
    [Fact]
    public async Task Creates_an_external_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent(identityAddress, "some-deletion-process-id");

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStarted, IdentityAddress.Parse(identityAddress), 1,
            new { identityDeletionProcessStartedDomainEvent.DeletionProcessId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
            A<IdentityAddress>.That.Matches(i => i.StringValue == identityAddress),
            ExternalEventType.IdentityDeletionProcessStarted,
            A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStartedDomainEventHandler(fakeDbContext, mockEventBus, A.Fake<ILogger<IdentityDeletionProcessStartedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Handle
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedDomainEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }
}
