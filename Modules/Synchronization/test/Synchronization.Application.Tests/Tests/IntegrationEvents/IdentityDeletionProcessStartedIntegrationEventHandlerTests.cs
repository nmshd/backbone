using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.IntegrationEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.IntegrationEvents;

public class IdentityDeletionProcessStartedIntegrationEventHandlerTests
{
    [Fact]
    public async Task Creates_external_events_after_consuming_integration_event()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStartedIntegrationEvent = new IdentityDeletionProcessStartedIntegrationEvent(identityAddress, "some-deletion-process-id");

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();
        var mockEventBus = A.Fake<IEventBus>();

        var externalEvent = new ExternalEvent(ExternalEventType.IdentityDeletionProcessStarted, IdentityAddress.Parse(identityAddress), 1,
            new { identityDeletionProcessStartedIntegrationEvent.DeletionProcessId });

        A.CallTo(() => fakeDbContext.CreateExternalEvent(
                A<IdentityAddress>.That.Matches(i => i.StringValue == identityAddress),
                ExternalEventType.IdentityDeletionProcessStarted,
                A<object>._)
        ).Returns(externalEvent);

        var handler = new IdentityDeletionProcessStartedIntegrationEventHandler(fakeDbContext, mockEventBus, A.Fake<ILogger<IdentityDeletionProcessStartedIntegrationEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedIntegrationEvent);

        // Handle
        A.CallTo(() => mockEventBus.Publish(
            A<ExternalEventCreatedIntegrationEvent>.That.Matches(e => e.Owner == externalEvent.Owner && e.EventId == externalEvent.Id))
        ).MustHaveHappenedOnceExactly();
    }
}
