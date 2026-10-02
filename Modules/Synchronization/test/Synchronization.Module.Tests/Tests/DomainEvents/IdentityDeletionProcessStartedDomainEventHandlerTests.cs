using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStarted;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Backbone.Modules.Synchronization.Module.Tests.Tests.DomainEvents;

public class IdentityDeletionProcessStartedDomainEventHandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event_if_initiator_is_someone_else()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent { Address = identityAddress, DeletionProcessId = "some-deletion-process-id" };

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
        var deletionProcessOwner = CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent
        { Address = deletionProcessOwner, DeletionProcessId = "some-deletion-process-id", Initiator = deletionProcessOwner };

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new IdentityDeletionProcessStartedDomainEventHandler(fakeDbContext, A.Fake<ILogger<IdentityDeletionProcessStartedDomainEventHandler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Assert
        A.CallTo(() => fakeDbContext.CreateExternalEvent(A<IdentityDeletionProcessStartedExternalEvent>._)).MustNotHaveHappened();
    }
}
