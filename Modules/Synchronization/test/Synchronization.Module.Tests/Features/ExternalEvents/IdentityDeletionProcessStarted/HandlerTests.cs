using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using IdentityDeletionProcessStartedSlice = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStarted;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.ExternalEvents.IdentityDeletionProcessStarted;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event_if_initiator_is_someone_else()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var identityDeletionProcessStartedDomainEvent = new IdentityDeletionProcessStartedDomainEvent { Address = identityAddress, DeletionProcessId = "some-deletion-process-id" };

        var fakeDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new IdentityDeletionProcessStartedSlice.Handler(fakeDbContext, A.Fake<ILogger<IdentityDeletionProcessStartedSlice.Handler>>());

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

        var handler = new IdentityDeletionProcessStartedSlice.Handler(fakeDbContext, A.Fake<ILogger<IdentityDeletionProcessStartedSlice.Handler>>());

        // Act
        await handler.Handle(identityDeletionProcessStartedDomainEvent);

        // Assert
        A.CallTo(() => fakeDbContext.CreateExternalEvent(A<IdentityDeletionProcessStartedExternalEvent>._)).MustNotHaveHappened();
    }
}
