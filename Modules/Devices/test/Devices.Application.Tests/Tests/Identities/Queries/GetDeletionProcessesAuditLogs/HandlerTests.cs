using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAuditLogs;

public class HandlerTests : AbstractTestsBase
{
    private readonly DevicesDbContext _devicesArrangeDbContext;
    private readonly DevicesDbContext _actDbContext;

    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        var connection = FakeDbContextFactory.CreateDbConnection();
        (_devicesArrangeDbContext, _, _) = CreateDbContexts(connection);
        (_, _, _actDbContext) = CreateDbContexts(connection);
    }

    [Fact]
    public async Task Gets_successfully()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);
        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        await _devicesArrangeDbContext.IdentityDeletionProcessAuditLogs.AddRangeAsync(identityDeletionProcessAuditLogs);
        await _devicesArrangeDbContext.SaveChangesAsync();

        await _devicesArrangeDbContext.Identities.Where(Identity.HasAddress(identity.Address)).ExecuteDeleteAsync();

        var identitiesRepository = new IdentitiesRepository(_actDbContext, A.Fake<UserManager<ApplicationUser>>());
        var handler = CreateHandler(identitiesRepository);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identityDeletionProcessAuditLogs.Count);
    }

    [Fact]
    public async Task Returns_empty_list_for_non_existent_address()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);
        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        await _devicesArrangeDbContext.IdentityDeletionProcessAuditLogs.AddRangeAsync(identityDeletionProcessAuditLogs);
        await _devicesArrangeDbContext.SaveChangesAsync();

        var identitiesRepository = new IdentitiesRepository(_actDbContext, A.Fake<UserManager<ApplicationUser>>());
        var handler = CreateHandler(identitiesRepository);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery("non-existent-identity-address"), CancellationToken.None);

        // Assert
        result.Should().HaveCount(0);
    }

    [Fact]
    public async Task Gets_for_multiple_deletion_processes()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);
        TestDataGenerator.CreateRejectedDeletionProcessFor(identity, identity.Devices.First().Id);
        TestDataGenerator.CreateApprovedDeletionProcessFor(identity, identity.Devices.First().Id);
        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        await _devicesArrangeDbContext.IdentityDeletionProcessAuditLogs.AddRangeAsync(identityDeletionProcessAuditLogs);
        await _devicesArrangeDbContext.SaveChangesAsync();

        await _devicesArrangeDbContext.Identities.Where(Identity.HasAddress(identity.Address)).ExecuteDeleteAsync();

        var identitiesRepository = new IdentitiesRepository(_actDbContext, A.Fake<UserManager<ApplicationUser>>());
        var handler = CreateHandler(identitiesRepository);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identityDeletionProcessAuditLogs.Count);
    }

    [Fact]
    public async Task Gets_for_deletion_process_in_status_deleting()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateDeletingDeletionProcessFor(identity, identity.Devices.First().Id);
        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        await _devicesArrangeDbContext.IdentityDeletionProcessAuditLogs.AddRangeAsync(identityDeletionProcessAuditLogs);
        await _devicesArrangeDbContext.SaveChangesAsync();

        await _devicesArrangeDbContext.Identities.Where(Identity.HasAddress(identity.Address)).ExecuteDeleteAsync();

        var identitiesRepository = new IdentitiesRepository(_actDbContext, A.Fake<UserManager<ApplicationUser>>());
        var handler = CreateHandler(identitiesRepository);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identityDeletionProcessAuditLogs.Count);
    }

    [Fact]
    public async Task Gets_for_deleted_identity()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateDeletingDeletionProcessFor(identity, identity.Devices.First().Id);
        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        await _devicesArrangeDbContext.IdentityDeletionProcessAuditLogs.AddRangeAsync(identityDeletionProcessAuditLogs);
        await _devicesArrangeDbContext.SaveChangesAsync();

        await _devicesArrangeDbContext.Identities.Where(Identity.HasAddress(identity.Address)).ExecuteDeleteAsync();

        var identitiesRepository = new IdentitiesRepository(_actDbContext, A.Fake<UserManager<ApplicationUser>>());
        var handler = CreateHandler(identitiesRepository);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identityDeletionProcessAuditLogs.Count);
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }

    private (DevicesDbContext arrangeContext, DevicesDbContext assertionContext, DevicesDbContext actContext)
        CreateDbContexts(SqliteConnection connection)
    {
        connection.Open();

        var options = new DbContextOptionsBuilder<DevicesDbContext>()
            .UseSqlite(connection)
            .Options;

        object[] args = [options];

        var context = (DevicesDbContext)Activator.CreateInstance(typeof(DevicesDbContext), args)!;
        context.Database.EnsureCreated();
        context.Dispose();

        var arrangeContext = (DevicesDbContext)Activator.CreateInstance(typeof(DevicesDbContext), args)!;
        var assertionContext = (DevicesDbContext)Activator.CreateInstance(typeof(DevicesDbContext), args)!;
        var actContext = (DevicesDbContext)Activator.CreateInstance(typeof(DevicesDbContext), args)!;

        return (arrangeContext, assertionContext, actContext);
    }
}
