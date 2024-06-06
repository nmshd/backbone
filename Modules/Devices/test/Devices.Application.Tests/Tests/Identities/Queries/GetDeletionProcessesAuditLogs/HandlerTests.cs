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
        var deletionProcess = identity.StartDeletionProcessAsSupport();
        identity.ApproveDeletionProcess(deletionProcess.Id, identity.Devices.ElementAt(0).Id);
        identity.CancelDeletionProcessAsSupport(deletionProcess.Id);
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
        var deletionProcess1 = identity.StartDeletionProcessAsSupport();
        identity.ApproveDeletionProcess(deletionProcess1.Id, identity.Devices.ElementAt(0).Id);
        identity.CancelDeletionProcessAsSupport(deletionProcess1.Id);

        var deletionProcess2 = identity.StartDeletionProcessAsSupport();
        identity.ApproveDeletionProcess(deletionProcess2.Id, identity.Devices.ElementAt(0).Id);
        identity.CancelDeletionProcessAsSupport(deletionProcess2.Id);
        var identityDeletionProcessAuditLogs = identity.DeletionProcesses.SelectMany(identityDeletionProcess => identityDeletionProcess.AuditLog).ToList();

        var handler = CreateHandler(new FindDeletionProcessAuditLogsByAddressStubRepository(identityDeletionProcessAuditLogs));

        // Act
        var result = await handler.Handle(new GetDeletionProcessesAuditLogsQuery(identity.Address), CancellationToken.None);

        // Assert
        result.ToList().Count.Should().Be(identityDeletionProcessAuditLogs.Count);
    }

    private Handler CreateHandler(IIdentitiesRepository identitiesRepository)
    {
        return new Handler(identitiesRepository);
    }
}
