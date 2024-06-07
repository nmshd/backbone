using Backbone.Modules.Devices.Application.Identities.Queries.GetDeletionProcessesAuditLogs;
using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.UnitTestTools.BaseClasses;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Devices.Application.Tests.Tests.Identities.Queries.GetDeletionProcessesAuditLogs;

public class HandlerTests : AbstractTestsBase
{
    [Fact]
    public async Task Gets_audit_logs_of_identity_with_address()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);

        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        var handler = CreateHandler(new FindDeletionProcessAuditLogsByAddressStubRepository(identityDeletionProcessAuditLogs));

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identityDeletionProcessAuditLogs.Count);
    }

    [Fact]
    public async Task Gets_audit_logs_of_identity_with_address_and_multiple_deletion_processes()
    {
        // Arrange
        var identity = TestDataGenerator.CreateIdentityWithOneDevice();
        TestDataGenerator.CreateCancelledDeletionProcessFor(identity);
        TestDataGenerator.CreateRejectedDeletionProcessFor(identity, identity.Devices.First().Id);
        TestDataGenerator.CreateApprovedDeletionProcessFor(identity, identity.Devices.First().Id);

        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        var handler = CreateHandler(new FindDeletionProcessAuditLogsByAddressStubRepository(identityDeletionProcessAuditLogs));

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.Should().HaveCount(identityDeletionProcessAuditLogs.Count);
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
