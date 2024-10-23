using Backbone.Modules.Synchronization.Application.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Application.Infrastructure;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Incoming.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.DomainEvents;

public class IdentityDeletionProcessStartedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event_if_initiator_is_someone_else()
    {
        // Arrange
        var identityAddress = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent(identityAddress, "some-deletion-process-id", null);

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new IdentityDeletionProcessStartedDomainEventHandler(fakeDbContext, A.Fake<ILogger<IdentityDeletionProcessStartedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Assert
        A.CallTo(() => fakeDbContext.CreateExternalEvent(A<IdentityDeletionProcessStartedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_nothing_if_initiator_is_deletion_process_owner()
    {
        // Arrange
        var deletionProcessOwner = TestDataGenerator.CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent(deletionProcessOwner, "some-deletion-process-id", deletionProcessOwner);

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new IdentityDeletionProcessStartedDomainEventHandler(fakeDbContext, A.Fake<ILogger<IdentityDeletionProcessStartedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Assert
        A.CallTo(() => fakeDbContext.CreateExternalEvent(A<IdentityDeletionProcessStartedExternalEvent>._)).MustNotHaveHappened();
    }
}
