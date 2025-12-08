using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.UnitTestTools.Shouldly.Extensions;
using FakeItEasy;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.FinalizeSyncRun;

public class HandlerTests : RequestHandlerTestsBase<SynchronizationDbContext>
{
    private readonly IdentityAddress _activeIdentity = CreateRandomIdentityAddress();
    private readonly DeviceId _activeDevice = CreateRandomDeviceId();

    public HandlerTests()
    {
        _arrangeContext.SaveEntity(new Domain.Entities.Datawallet(new Domain.Entities.Datawallet.DatawalletVersion(1), _activeIdentity));
    }

    [Fact]
    public async Task Cannot_be_finalized_by_other_identity()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(CreateRandomIdentityAddress())
            .CreatedByDevice(CreateRandomDeviceId())
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var acting = async () => await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id }, CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<NotFoundException>().ShouldContainMessage("SyncRun");
    }

    [Fact]
    public async Task Cannot_finalize_when_no_active_sync_run_exists()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Finalized()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var acting = async () =>
            await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, DatawalletModifications = [], ExternalEventResults = [] }, CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<OperationFailedException>().ShouldHaveErrorCode("error.platform.validation.syncRun.syncRunAlreadyFinalized");
    }

    [Fact]
    public async Task Finalize_sync_run_without_results_succeeds()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, ExternalEventResults = [], DatawalletModifications = [] }, CancellationToken.None);

        // Assert
        // No Exception means success
    }

    [Fact]
    public async Task Item_results_with_error_code_delete_sync_run_reference()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var item = ExternalEventBuilder.Build().WithOwner(_activeIdentity).AssignedToSyncRun(syncRun).Create();
        _arrangeContext.SaveEntity(item);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var results = new List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> { new(item.Id, "some-random-error-code") };

        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, DatawalletModifications = [], ExternalEventResults = results }, CancellationToken.None);
        // Assert
        var externalEvent = _assertionContext.ExternalEvents.First(i => i.Id == item.Id);
        externalEvent.SyncRunId.ShouldBeNull();
    }

    [Fact]
    public async Task Missing_results_lead_to_SyncErrors()
    {
        // Arrange
        var item1 = ExternalEventBuilder.Build().WithOwner(_activeIdentity).WithIndex(0).Create();
        var item2 = ExternalEventBuilder.Build().WithOwner(_activeIdentity).WithIndex(1).Create();
        var items = _arrangeContext.SaveEntities(item1, item2);

        var syncRun = SyncRunBuilder
            .Build()
            .WithExternalEvents(items)
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var eventResults = new List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> { new(item1.Id) { ExternalEventId = item1.Id } };
        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, ExternalEventResults = eventResults }, CancellationToken.None);

        // Assert
        _assertionContext.SyncErrors.ShouldContain(e => e.ExternalEventId == item2.Id && e.ErrorCode == "notProcessed");
    }

    [Fact]
    public async Task Passed_DatawalletModifications_are_saved_to_database()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var datawalletModifications = new List<PushDatawalletModificationItem>
        {
            new()
            {
                Type = DatawalletModificationDTO.DatawalletModificationType.Create,
                Collection = "someArbitraryCollection",
                EncryptedPayload = [0],
                ObjectIdentifier = "someArbitraryObjectIdentifier",
                PayloadCategory = "someArbitraryObjectProperty",
                DatawalletVersion = 1
            }
        };

        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, DatawalletModifications = datawalletModifications }, CancellationToken.None);

        // Assert
        _assertionContext.DatawalletModifications.ShouldHaveCount(1);
    }

    [Fact]
    public async Task Successful_item_results_do_not_delete_sync_run_reference()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var item = ExternalEventBuilder.Build().WithOwner(_activeIdentity).AssignedToSyncRun(syncRun).Create();
        _arrangeContext.SaveEntity(item);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var results = new List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> { new(item.Id) };

        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, ExternalEventResults = results }, CancellationToken.None);
        // Assert
        var externalEvent = _assertionContext.ExternalEvents.First(i => i.Id == item.Id);
        externalEvent.SyncRunId.ShouldBe(syncRun.Id);
    }

    [Fact]
    public async Task Sync_errors_for_item_results_with_error_code_are_created()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(_activeDevice)
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var item = ExternalEventBuilder.Build().WithOwner(_activeIdentity).AssignedToSyncRun(syncRun).Create();
        _arrangeContext.SaveEntity(item);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        var results = new List<FinalizeExternalEventSyncSyncRunCommand.ExternalEventResult> { new(item.Id, "some-random-error-code") };
        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id, ExternalEventResults = results }, CancellationToken.None);

        // Assert
        _assertionContext.SyncErrors
            .ShouldContain(e => e.ExternalEventId == item.Id && e.ErrorCode == "some-random-error-code");
    }

    [Fact]
    public async Task Sync_run_can_only_be_finalized_by_creator_device()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(CreateRandomDeviceId())
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        Func<Task> acting = async () => await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand { SyncRunId = syncRun.Id }, CancellationToken.None);

        // Assert
        await acting.ShouldThrowAsync<OperationFailedException>().ShouldHaveErrorCode("error.platform.validation.syncRun.cannotFinalizeSyncRunStartedByAnotherDevice");
    }

    #region CreateHandler

    private Handler CreateHandler(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => userContext.GetDeviceId()).Returns(activeDevice);

        return new Handler(_actContext, userContext);
    }

    #endregion
}
