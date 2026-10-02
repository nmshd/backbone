using Backbone.Modules.Devices.Contracts.DomainEvents;
using Backbone.Modules.Synchronization.Abstractions;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using IdentityDeletionProcessStatusChangedSlice = Backbone.Modules.Synchronization.Module.Features.ExternalEvents.IdentityDeletionProcessStatusChanged;

namespace Backbone.Modules.Synchronization.Module.Tests.Features.ExternalEvents.IdentityDeletionProcessStatusChanged;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Creates_an_external_event_if_initiator_is_someone_else()
    {
        // Arrange
        var deletionProcessOwner = CreateRandomIdentityAddress();
        var identityDeletionProcessStatusChangedDomainEvent = new IdentityDeletionProcessStatusChangedDomainEvent
        { DeletionProcessOwner = deletionProcessOwner, DeletionProcessId = "someDeletionProcessId" };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new IdentityDeletionProcessStatusChangedSlice.Handler(mockDbContext,
            A.Fake<ILogger<IdentityDeletionProcessStatusChangedSlice.Handler>>());

        // Act
        await handler.Handle(identityDeletionProcessStatusChangedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<IdentityDeletionProcessStatusChangedExternalEvent>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Does_nothing_if_initiator_is_deletion_process_owner()
    {
        // Arrange
        var deletionProcessOwner = CreateRandomIdentityAddress();
        var identityDeletionProcessStatusChangedDomainEvent = new IdentityDeletionProcessStatusChangedDomainEvent
        { DeletionProcessOwner = deletionProcessOwner, DeletionProcessId = "someDeletionProcessId", Initiator = deletionProcessOwner };

        var mockDbContext = A.Fake<ISynchronizationDbContext>();

        var handler = new IdentityDeletionProcessStatusChangedSlice.Handler(mockDbContext,
            A.Fake<ILogger<IdentityDeletionProcessStatusChangedSlice.Handler>>());

        // Act
        await handler.Handle(identityDeletionProcessStatusChangedDomainEvent);

        // Assert
        A.CallTo(() => mockDbContext.CreateExternalEvent(A<IdentityDeletionProcessStatusChangedExternalEvent>._)).MustNotHaveHappened();
    }
}
