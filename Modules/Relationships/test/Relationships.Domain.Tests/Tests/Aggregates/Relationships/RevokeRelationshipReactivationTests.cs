using Backbone.BuildingBlocks.Domain;
using Backbone.Modules.Relationships.Domain.Aggregates.Relationships;
using Backbone.Modules.Relationships.Domain.Tests.Extensions;
using Backbone.Tooling;
using FluentAssertions;
using Xunit;
using static Backbone.Modules.Relationships.Domain.Tests.TestHelpers.TestData;

namespace Backbone.Modules.Relationships.Domain.Tests.Tests.Aggregates.Relationships;
public class RevokeRelationshipReactivationTests
{
    [Fact]
    public void Terminating_relationship_creates_an_audit_log_entry()
    {
        // Arrange
        SystemTime.Set("2000-01-01");
        var relationship = CreateTerminatedRelationshipWithReactivationRequest();

        // Act
        relationship.RevokeReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        relationship.AuditLog.Should().HaveCount(5);

        var auditLogEntry = relationship.AuditLog.Last();

        auditLogEntry.Id.Should().NotBeNull();
        auditLogEntry.Reason.Should().Be(RelationshipAuditLogEntryReason.RevocationOfReactivation);
        auditLogEntry.OldStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.NewStatus.Should().Be(RelationshipStatus.Terminated);
        auditLogEntry.CreatedBy.Should().Be(IDENTITY_1);
        auditLogEntry.CreatedByDevice.Should().Be(DEVICE_1);
        auditLogEntry.CreatedAt.Should().Be(DateTime.Parse("2000-01-01"));
    }

    [Fact]
    public void Can_only_revoke_reactivation_of_relationship_when_reactivation_has_been_requested()
    {
        // Arrange
        var relationship = CreateTerminatedRelationship();

        // Act
        var acting = () => relationship.RevokeReactivation(IDENTITY_1, DEVICE_1);

        // Assert
        acting.Should().Throw<DomainException>().WithError(
            "error.platform.validation.relationshipRequest.noOpenReactivationRequest"
        );
    }
}
