﻿using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Backbone.Modules.Devices.Infrastructure.Persistence.Database;
using Backbone.Modules.Devices.Infrastructure.Persistence.Repository;
using Backbone.UnitTestTools.BaseClasses;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.TestDoubles.Fakes;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAuditLogs;

public class HandlerTests : AbstractTestsBase
{
    private readonly DevicesDbContext _arrangeDbContext;
    private readonly DevicesDbContext _actDbContext;

    public HandlerTests()
    {
        AssertionScope.Current.FormattingOptions.MaxLines = 1000;

        (_arrangeDbContext, _actDbContext, _) = FakeDbContextFactory.CreateDbContexts<DevicesDbContext>();
    }

    [Fact]
    public async Task Gets_successfully()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);

        await _arrangeDbContext.SaveEntity(identity);
        await _arrangeDbContext.RemoveEntity(identity);

        var handler = CreateHandler(_actDbContext);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identity.DeletionProcesses.SelectMany(d => d.AuditLog).Count());
    }

    [Fact]
    public async Task Returns_empty_list_for_non_existent_address()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);

        await _arrangeDbContext.SaveEntity(identity);

        var handler = CreateHandler(_actDbContext);

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

        await _arrangeDbContext.SaveEntity(identity);
        await _arrangeDbContext.RemoveEntity(identity);

        var handler = CreateHandler(_actDbContext);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identity.DeletionProcesses.SelectMany(d => d.AuditLog).Count());
    }

    [Fact]
    public async Task Gets_for_deletion_process_in_status_deleting()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateDeletingDeletionProcessFor(identity, identity.Devices.First().Id);

        await _arrangeDbContext.SaveEntity(identity);
        await _arrangeDbContext.RemoveEntity(identity);

        var handler = CreateHandler(_actDbContext);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identity.DeletionProcesses.SelectMany(d => d.AuditLog).Count());
    }

    [Fact]
    public async Task Gets_for_deleted_identity()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateDeletingDeletionProcessFor(identity, identity.Devices.First().Id);

        await _arrangeDbContext.SaveEntity(identity);
        await _arrangeDbContext.RemoveEntity(identity);

        var handler = CreateHandler(_actDbContext);

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identity.DeletionProcesses.SelectMany(d => d.AuditLog).Count());
    }

    private static Handler CreateHandler(DevicesDbContext actDbContext)
    {
        var repository = new IdentitiesRepository(actDbContext, A.Fake<UserManager<ApplicationUser>>());
        return new Handler(repository);
    }
}
