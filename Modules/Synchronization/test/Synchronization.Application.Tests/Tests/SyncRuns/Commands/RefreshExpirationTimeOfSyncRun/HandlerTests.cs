using Backbone.BuildingBlocks.Application.Abstractions.Exceptions;
using Backbone.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Application.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;
using Backbone.Modules.Synchronization.Infrastructure.Persistence.Database;
using Backbone.Tooling;
using Backbone.UnitTestTools.BaseClasses;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Synchronization.Application.Tests.Tests.SyncRuns.Commands.RefreshExpirationTimeOfSyncRun;

public class HandlerTests : RequestHandlerTestsBase<SynchronizationDbContext>
{
    [Fact]
    public async Task Cannot_refresh_expiration_time_of_sync_run_created_by_other_device()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();
        var handler = CreateHandler(activeIdentity, activeDevice);

        var syncRun = SyncRunBuilder.Build().CreatedBy(activeIdentity).CreatedByDevice(TestDataGenerator.CreateRandomDeviceId()).Create();
        _arrangeContext.SaveEntity(syncRun);


        // Act
        Func<Task> acting = async () => await handler.Handle(new RefreshExpirationTimeOfSyncRunCommand(syncRun.Id), CancellationToken.None);


        // Assert
        await acting.Should().ThrowAsync<OperationFailedException>().WithErrorCode("error.platform.validation.syncRun.cannotRefreshExpirationTimeOfSyncRunStartedByAnotherDevice");
    }

    [Fact]
    public async Task Cannot_refresh_expiration_time_of_sync_run_created_by_other_identity()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();
        var handler = CreateHandler(activeIdentity, activeDevice);

        var syncRun = SyncRunBuilder.Build().CreatedBy(TestDataGenerator.CreateRandomIdentityAddress()).CreatedByDevice(TestDataGenerator.CreateRandomDeviceId()).Create();
        _arrangeContext.SaveEntity(syncRun);


        // Act
        Func<Task> acting = async () => await handler.Handle(new RefreshExpirationTimeOfSyncRunCommand(syncRun.Id), CancellationToken.None);


        // Assert
        await acting.Should().ThrowAsync<NotFoundException>().WithMessage("*SyncRun*");
    }

    [Fact]
    public async Task Refresh_expiration_time()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();
        var handler = CreateHandler(activeIdentity, activeDevice);

        var syncRun = SyncRunBuilder.Build().CreatedBy(activeIdentity).CreatedByDevice(activeDevice).Create();
        _arrangeContext.SaveEntity(syncRun);

        var utcNow = DateTime.UtcNow;
        SystemTime.Set(utcNow);


        // Act
        var response = await handler.Handle(new RefreshExpirationTimeOfSyncRunCommand(syncRun.Id), CancellationToken.None);


        // Assert
        response.ExpiresAt.Should().BeAfter(utcNow);
    }

    [Fact]
    public async Task Refresh_expiration_time_of_expired_sync_run()
    {
        // Arrange
        var activeIdentity = TestDataGenerator.CreateRandomIdentityAddress();
        var activeDevice = TestDataGenerator.CreateRandomDeviceId();
        var handler = CreateHandler(activeIdentity, activeDevice);

        var utcNow = DateTime.UtcNow;
        SystemTime.Set(utcNow);

        var syncRun = SyncRunBuilder.Build().CreatedBy(activeIdentity).ExpiresAt(utcNow.AddDays(-5)).CreatedByDevice(activeDevice).Create();
        _arrangeContext.SaveEntity(syncRun);


        // Act
        var response = await handler.Handle(new RefreshExpirationTimeOfSyncRunCommand(syncRun.Id), CancellationToken.None);


        // Assert
        response.ExpiresAt.Should().BeAfter(utcNow);
    }

    #region CreateHandler

    private Handler CreateHandler(IdentityAddress activeIdentity, DeviceId createdByDevice)
    {
        var userContext = A.Fake<IUserContext>();
        A.CallTo(() => userContext.GetAddress()).Returns(activeIdentity);
        A.CallTo(() => userContext.GetDeviceId()).Returns(createdByDevice);

        return new Handler(_actContext, userContext);
    }

    #endregion
}
