using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.AutoMapper;
using Backbone.Modules.Synchronization.Application.Datawallets.DTOs;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.FinalizeSyncRun;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.FinalizeSyncRun;

public class HandlerTests : RequestHandlerTestsBase<SynchronizationDbContext>
{
    private readonly IdentityAddress _activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
    private readonly DeviceId _activeDevice = TestDataGenerator.CreateRandomDeviceId();

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
            .CreatedBy(TestDataGenerator.CreateRandomIdentityAddress())
            .CreatedByDevice(TestDataGenerator.CreateRandomDeviceId())
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        Func<Task> acting = async () => await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<NotFoundException>().WithMessage("*SyncRun*");
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
        Func<Task> acting = async () => await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<OperationFailedException>().WithErrorCode("error.platform.validation.syncRun.syncRunAlreadyFinalized");
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
        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id), CancellationToken.None);

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

        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id, results), CancellationToken.None);
        // Assert
        var externalEvent = _assertionContext.ExternalEvents.First(i => i.Id == item.Id);
        externalEvent.SyncRunId.Should().BeNull();
    }

    [Fact]
    public async Task Missing_results_lead_to_SyncErrors()
    {
        // Arrange
        var item1 = ExternalEventBuilder.Build().WithOwner(_activeIdentity).Create();
        var item2 = ExternalEventBuilder.Build().WithOwner(_activeIdentity).Create();
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
        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id, eventResults), CancellationToken.None);

        // Assert
        _assertionContext.SyncErrors.Should().Contain(e => e.ExternalEventId == item2.Id).Which.ErrorCode.Should().Be("notProcessed");
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

        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id, datawalletModifications), CancellationToken.None);

        // Assert
        _assertionContext.DatawalletModifications.Should().HaveCount(1);
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

        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id, results), CancellationToken.None);
        // Assert
        var externalEvent = _assertionContext.ExternalEvents.First(i => i.Id == item.Id);
        externalEvent.SyncRunId.Should().Be(syncRun.Id);
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
        await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id, results), CancellationToken.None);

        // Assert
        _assertionContext.SyncErrors
            .Should().Contain(e => e.ExternalEventId == item.Id)
            .Which.ErrorCode.Should().Be("some-random-error-code");
    }

    [Fact]
    public async Task Sync_run_can_only_be_finalized_by_creator_device()
    {
        // Arrange
        var syncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .CreatedByDevice(TestDataGenerator.CreateRandomDeviceId())
            .Running()
            .Create();
        _arrangeContext.SaveEntity(syncRun);

        var handler = CreateHandler(_activeIdentity, _activeDevice);

        // Act
        Func<Task> acting = async () => await handler.Handle(new FinalizeExternalEventSyncSyncRunCommand(syncRun.Id), CancellationToken.None);

        // Assert
        await acting.Should().ThrowAsync<OperationFailedException>().WithErrorCode("error.platform.validation.syncRun.cannotFinalizeSyncRunStartedByAnotherDevice");
    }

    #region CreateHandler

    private Handler CreateHandler(IdentityAddress activeIdentity, DeviceId activeDevice)
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => userContext.GetDeviceId()).Returns(activeDevice);

        var mapper = AutoMapperProfile.CreateMapper();

        var eventBus = A.Fake<IEventBus>();

        return new Handler(_actContext, userContext, mapper, eventBus);
    }

    #endregion
}
