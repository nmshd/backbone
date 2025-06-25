using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.EventBus;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.StartSyncRun;
using Backbone.Modules.Synchronization.Application.SyncRuns.DTOs;
using Backbone.Modules.Synchronization.Domain.Entities.Sync;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.Shouldly.Extensions;
using FakeItEasy;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.StartSyncRun;

public class HandlerTests : AbstractTestsBase
{
    private const int DATAWALLET_VERSION = 1;
    private readonly SynchronizationDbContext _actContext;
    private readonly DeviceId _activeDevice = CreateRandomDeviceId();
    private readonly IdentityAddress _activeIdentity = CreateRandomIdentityAddress();
    private readonly SynchronizationDbContext _arrangeContext;
    private readonly SynchronizationDbContext _assertionContext;
    private readonly DbContextOptions<SynchronizationDbContext> _dbOptions;
    private readonly IEventBus _eventBus;

    public HandlerTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        _dbOptions = new DbContextOptionsBuilder<SynchronizationDbContext>().UseSqlite(connection).Options;
        _eventBus = A.Dummy<IEventBus>();

        var setupContext = new SynchronizationDbContext(_dbOptions, _eventBus);
        setupContext.Database.EnsureCreated();
        setupContext.Dispose();

        _arrangeContext = CreateDbContext();
        _actContext = CreateDbContext();
        _assertionContext = CreateDbContext();

        _arrangeContext.SaveEntity(new Domain.Entities.Datawallet(new Domain.Entities.Datawallet.DatawalletVersion(DATAWALLET_VERSION), _activeIdentity));
    }

    [Fact]
    public async Task Start_a_sync_run()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);

        var externalEvent = ExternalEventBuilder.Build().WithOwner(_activeIdentity).Create();
        _arrangeContext.SaveEntity(externalEvent);


        // Act
        var response = await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION }, CancellationToken.None);


        // Assert
        response.Status.ShouldBe(StartSyncRunStatus.Created);
        response.SyncRun.ShouldNotBeNull();
    }

    [Fact]
    public async Task Starting_two_sync_runs_parallelly_leads_to_error_for_one_call()
    {
        // Arrange
        var externalEvent = ExternalEventBuilder.Build().WithOwner(_activeIdentity).Create();
        _arrangeContext.SaveEntity(externalEvent);

        // By adding a save-delay to one of the calls, we can ensure that the second one will finish first, and therefore the first one
        // will definitely run into an error regarding the duplicate database index.
        var handlerWithDelayedSave = CreateHandlerWithDelayedSave(TimeSpan.FromMilliseconds(200));
        var handlerWithImmediateSave = CreateHandlerWithDelayedSave(TimeSpan.FromMilliseconds(50));


        // Act
        var taskWithImmediateSave = handlerWithImmediateSave.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION },
            CancellationToken.None);

        var taskWithDelayedSave = handlerWithDelayedSave.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION },
            CancellationToken.None);

        var handleWithDelayedSave = () => taskWithDelayedSave;
        var handleWithImmediateSave = () => taskWithImmediateSave;


        // Assert
        await handleWithDelayedSave
            .ShouldThrowAsync<OperationFailedException>()
            .ShouldContainMessage("Another sync run is currently active.")
            .ShouldHaveErrorCode("error.platform.validation.syncRun.cannotStartSyncRunWhenAnotherSyncRunIsRunning");

        await handleWithImmediateSave.ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Cannot_start_sync_run_when_another_one_is_running()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);

        var aRunningSyncRun = SyncRunBuilder.Build().CreatedBy(_activeIdentity).Create();
        _arrangeContext.SaveEntity(aRunningSyncRun);


        // Act
        Func<Task> acting = async () =>
            await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION }, CancellationToken.None);


        // Assert
        await acting.ShouldThrowAsync<OperationFailedException>().ShouldHaveErrorCode("error.platform.validation.syncRun.cannotStartSyncRunWhenAnotherSyncRunIsRunning");
    }

    [Fact]
    public async Task No_sync_items_with_max_error_count_are_added()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);

        var itemWithoutErrors = ExternalEventBuilder.Build().WithOwner(_activeIdentity).WithIndex(0).Create();
        _arrangeContext.SaveEntity(itemWithoutErrors);

        var itemWithMaxErrorCount = ExternalEventBuilder.Build().WithOwner(_activeIdentity).WithIndex(1).WithMaxErrorCount().Create();
        _arrangeContext.SaveEntity(itemWithMaxErrorCount);


        // Act
        var response = await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION }, CancellationToken.None);


        // Assert
        var itemsOfSyncRun = _assertionContext.ExternalEvents.Where(i => i.SyncRunId! == response.SyncRun!.Id);
        itemsOfSyncRun.ShouldContain(i => i.Id == itemWithoutErrors.Id);
        itemsOfSyncRun.ShouldNotContain(i => i.Id == itemWithMaxErrorCount.Id);
    }

    [Fact]
    public async Task No_sync_run_is_started_when_no_new_sync_items_exist()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);


        // Act
        var response = await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION }, CancellationToken.None);


        // Assert
        response.Status.ShouldBe(StartSyncRunStatus.NoNewEvents);
        response.SyncRun.ShouldBeNull();
    }

    [Fact]
    public async Task Only_sync_items_of_active_identity_are_added()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);

        var itemOfActiveIdentity = ExternalEventBuilder.Build().WithOwner(_activeIdentity).Create();
        _arrangeContext.SaveEntity(itemOfActiveIdentity);

        var itemOfOtherIdentity = ExternalEventBuilder.Build().WithOwner(CreateRandomIdentityAddress()).Create();
        _arrangeContext.SaveEntity(itemOfOtherIdentity);


        // Act
        var response = await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = 1 }, CancellationToken.None);


        // Assert
        var itemsOfSyncRun = _assertionContext.ExternalEvents.Where(i => i.SyncRunId! == response.SyncRun!.Id);
        itemsOfSyncRun.ShouldContain(i => i.Id == itemOfActiveIdentity.Id);
        itemsOfSyncRun.ShouldNotContain(i => i.Id == itemOfOtherIdentity.Id);
    }

    [Fact]
    public async Task Only_unsynced_sync_items_are_added()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);

        var unsyncedItem = _arrangeContext.SaveEntity(ExternalEventBuilder.Build().WithOwner(_activeIdentity).WithIndex(0).Create());
        var syncedItem = _arrangeContext.SaveEntity(ExternalEventBuilder.Build().WithOwner(_activeIdentity).WithIndex(1).AlreadySynced().Create());


        // Act
        var response = await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION }, CancellationToken.None);


        // Assert
        var itemsOfSyncRun = _assertionContext.ExternalEvents.Where(i => i.SyncRunId! == response.SyncRun!.Id);
        itemsOfSyncRun.ShouldContain(i => i.Id == unsyncedItem.Id);
        itemsOfSyncRun.ShouldNotContain(i => i.Id == syncedItem.Id);
    }

    [Fact]
    public async Task Start_a_sync_run_when_already_running_sync_run_is_expired()
    {
        // Arrange
        var handler = CreateHandler(_activeIdentity);

        var externalEvent = ExternalEventBuilder
            .Build()
            .WithOwner(_activeIdentity)
            .Create();
        _arrangeContext.SaveEntity(externalEvent);

        var expiredSyncRun = SyncRunBuilder
            .Build()
            .CreatedBy(_activeIdentity)
            .ExpiresAt(SystemTime.UtcNow.AddDays(-5))
            .WithExternalEvents(new List<ExternalEvent> { externalEvent })
            .Create();
        _arrangeContext.SaveEntity(expiredSyncRun);


        // Act
        var response = await handler.Handle(new StartSyncRunCommand { Type = SyncRunDTO.SyncRunType.ExternalEventSync, SupportedDatawalletVersion = DATAWALLET_VERSION }, CancellationToken.None);


        // Assert
        response.Status.ShouldBe(StartSyncRunStatus.Created);
        response.SyncRun.ShouldNotBeNull();

        var canceledSyncRun = _assertionContext.SyncRuns.First(s => s.Id == expiredSyncRun.Id);
        canceledSyncRun.FinalizedAt.ShouldNotBeNull();

        var externalEventOfCanceledSyncRun = _assertionContext.ExternalEvents.First(i => i.Id == externalEvent.Id);
        externalEventOfCanceledSyncRun.SyncRunId?.Value.ShouldBe(response.SyncRun!.Id);
        externalEventOfCanceledSyncRun.SyncErrorCount.ShouldBe((byte)1);
    }

    #region CreateHandler

    private SynchronizationDbContext CreateDbContext()
    {
        return new SynchronizationDbContext(_dbOptions, _eventBus);
    }

    private Handler CreateHandlerWithDelayedSave(TimeSpan delay)
    {
        return CreateHandler(_activeIdentity, _activeDevice, CreateDbContextWithDelayedSave(delay));
    }

    private ApplicationDbContextWithDelayedSave CreateDbContextWithDelayedSave(TimeSpan delay)
    {
        return new ApplicationDbContextWithDelayedSave(_dbOptions, delay, _eventBus);
    }

    private Handler CreateHandler(IdentityAddress activeIdentity)
    {
        var activeDevice = CreateRandomDeviceId();
        var handler = CreateHandler(activeIdentity, activeDevice);
        return handler;
    }

    private Handler CreateHandler(IdentityAddress activeIdentity, DeviceId createdByDevice, SynchronizationDbContext? dbContext = null)
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => userContext.GetDeviceId()).Returns(createdByDevice);

        return new Handler(dbContext ?? _actContext, userContext);
    }

    #endregion
}
